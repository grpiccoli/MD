using ConsultaMD.Areas.MDs.Models;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.MDs.Controllers
{
    [Area("MDs")]
    public class MDashController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEvent _eventService;
        private readonly UserManager<ApplicationUser> _userManager;
        public MDashController(UserManager<ApplicationUser> userManager, 
            IEvent eventService,
            ApplicationDbContext context)
        {
            _eventService = eventService;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var timeSlots = _context.TimeSlots
                .Include(u => u.Reservation)
                .Include(u => u.Agenda)
                    .ThenInclude(p => p.AgendaEvent)
                        .ThenInclude(d => d.MediumDoctor)
                            .ThenInclude(m => m.Doctor)
                                .ThenInclude(m => m.Natural)
                                    .ThenInclude(m => m.ApplicationUser)
                .Where(t => t.Agenda.AgendaEvent.MediumDoctor.Doctor
                .Natural.ApplicationUser.UserName == User.Identity.Name);
            var model = new SummaryVM
            {
                TimeSlots = timeSlots.Count(),
                TakenTimeSlots = timeSlots.Count(t => t.Reservation != null)
            };
            return View(model);
        }
        public async Task<IActionResult> MDWaitingRoom()
        {
            var user = await _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);

            var now = DateTime.Now;
            var patients = _context.Reservations
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Natural)
                        .ThenInclude(p => p.ApplicationUser)
                .Include(a => a.TimeSlot)
                    .ThenInclude(p => p.Agenda)
                .Where(a => a.TimeSlot.Agenda.StartTime <= now.AddMinutes(10) && a.TimeSlot.Agenda.EndTime >= now.AddMinutes(-10))
                .Select(a => new MDWaitingRoomPatientVM { 
                    Age = a.Patient.Natural.Age(),
                    Name = a.Patient.Natural.GetShortName(),
                    Birth = a.Patient.Natural.Birth,
                    Email = a.Patient.Natural.ApplicationUser.Email,
                    Nationality = a.Patient.Natural.Nationality,
                    Sex = a.Patient.Natural.Sex,
                    Phone = a.Patient.Natural.ApplicationUser.PhoneNumber,
                    IsInLine = a.Arrived,
                    ArrivalTime = a.Arrival,
                    Insurance = a.Patient.Insurance,
                    Id = a.Id,
                    IsConfirmed = a.Confirmed,
                    IsIn = a.MedicalAttentionId == user.Person.Doctor.MedicalAttentionId
                });
            var model = new MDWaitingRoomVM
            {
                Attending = patients.SingleOrDefault(p => p.IsIn),
                Waiting = patients.Where(p => p.IsInLine && !p.IsIn),
                Confirmed = patients.Where(p => !p.IsInLine && p.IsConfirmed)
            };
            return View(model);
        }
        public async Task<IActionResult> AgendaEvents()
        {
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.MedicalAttentionMedium)
                                .ThenInclude(m => m.Place)
                                    .ThenInclude(m => m.Commune)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.AgendaEvents)
                                .ThenInclude(m => m.EventDayWeeks)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.AgendaEvents)
                                .ThenInclude(m => m.Agendas)
                                    .ThenInclude(m => m.TimeSlots)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);
            var model = user.Person.Doctor.MediumDoctors
                .SelectMany(m => m.AgendaEvents.Select(a => 
            new AgendaEventVM
            {
                Location = $"{m.MedicalAttentionMedium.Place.Name}, {m.MedicalAttentionMedium.Place.Commune.Name}",
                StartTime = a.StartDateTime,
                EndTime = a.EndDateTime,
                Duration = a.Duration,
                TimeSlots = a.Agendas.SelectMany(ag => ag.TimeSlots),
                EventDayWeeks = a.EventDayWeeks
            }));
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AgendaJson()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var random = new Random();
            var model = _context.TimeSlots
                .Include(t => t.Reservation)
                    .ThenInclude(r => r.Patient)
                        .ThenInclude(r => r.Natural)
                .Include(t => t.Agenda)
                    .ThenInclude(r => r.AgendaEvent)
                        .ThenInclude(r => r.MediumDoctor)
                            .ThenInclude(r => r.Doctor)
                .Where(i => i.Agenda.AgendaEvent.MediumDoctor.Doctor.NaturalId == user.PersonId)
                .Select(m => new AgendaJsonVM
                {
                    Start = m.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                    End = m.EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                    Text = m.Reservation == null ? "Libre" : m.Reservation.Patient.Natural.GetShortName(),
                    Color = string.Format(CultureInfo.InvariantCulture, "#{0:X6}", random.Next(0x1000000))
                    //m.Agenda.AgendaEvent.MediumDoctor.Color
                });
            StringBuilder sb = new StringBuilder();
            sb.Append("try { undefined(");
            sb.Append(JsonConvert.SerializeObject(model));
            sb.Append("); } catch (ex) { }");
            return Ok(sb.ToString());
        }
        //GET
        public async Task<IActionResult> AddAgenda()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var model = new AddAgendaVM
            {
                SelectorList = JsonConvert.SerializeObject(
                _context.MediumDoctors
                .Include(i => i.MedicalAttentionMedium)
                    .ThenInclude(m => m.Place)
                        .ThenInclude(m => m.Commune)
                .Where(i => i.Doctor.NaturalId == user.PersonId)
                .Select(i =>
                new MsSelect
                {
                    value = i.Id,
                    text = $"{i.MedicalAttentionMedium.Place.Name}, {i.MedicalAttentionMedium.Place.Commune.Name}"
                }))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddAgenda(
            [Bind("MediumDoctorId, StartTime, EndTime, Duration, HasOverTime, Frequency, Days")] 
        AddAgendaVM model) {
            if (ModelState.IsValid && model != null)
            {
                if(model.StartTime <= model.EndTime.AddMinutes(model.Duration))
                {
                    var agendaEvent = new AgendaEvent
                    {
                        Duration = new TimeSpan(0, 0, model.Duration, 0, 0),
                        StartDateTime = model.StartTime,
                        EndDateTime = model.EndTime,
                        MediumDoctorId = model.MediumDoctorId,
                        Frequency = model.Frequency
                    };
                    agendaEvent.DaysOfWeek.UnionWith(model.DaysOfWeek);
                    await _eventService.AddEvent(agendaEvent).ConfigureAwait(false);
                    return RedirectToAction(nameof(Agenda));
                }
                ModelState.AddModelError(string.Empty, "Hora de Inicio debe ser menor o igual a la hora de término más la duración");
            }
            return View(model);
        }
        public async Task<IActionResult> Agenda()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var random = new Random();
            var model = _context.TimeSlots
                .Include(t => t.Reservation)
                    .ThenInclude(r => r.Patient)
                        .ThenInclude(r => r.Natural)
                .Include(t => t.Agenda)
                    .ThenInclude(r => r.AgendaEvent)
                        .ThenInclude(r => r.MediumDoctor)
                            .ThenInclude(r => r.Doctor)
                .Include(t => t.Agenda)
                    .ThenInclude(r => r.AgendaEvent)
                        .ThenInclude(r => r.MediumDoctor)
                            .ThenInclude(r => r.MedicalAttentionMedium)
                                .ThenInclude(r => r.Place)
                                    .ThenInclude(r => r.Commune)
                .Where(i => i.Agenda.AgendaEvent.MediumDoctor.Doctor.NaturalId == user.PersonId)
                .Select(m => new AgendaJsonVM
                {
                    Start = m.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                    End = m.EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                    Text = m.Reservation == null ? "Libre" : m.Reservation.Patient.Natural.GetShortName(),
                    Color = string.Format(CultureInfo.InvariantCulture, "#{0:X6}", random.Next(0x1000000)),
                    //m.Agenda.AgendaEvent.MediumDoctor.Color
                    Location = $"{m.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Name}, {m.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Commune.Name}"
                });

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAgenda(int id)
        {
            await _eventService.DeleteEvent(id).ConfigureAwait(false);
            return RedirectToAction(nameof(AgendaEvents));
        }
        [HttpPost]
        public async Task<IActionResult> DisableAgenda(int id)
        {
            await _eventService.DisableEvent(id).ConfigureAwait(false);
            return RedirectToAction(nameof(AgendaEvents));
        }
        public async Task<IActionResult> Locations()
        {
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.MedicalAttentionMedium)
                                .ThenInclude(m => m.Place)
                                    .ThenInclude(p => p.Commune)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.InsuranceLocations)
                                .ThenInclude(i => i.InsuranceAgreement)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);
            var model = user.Person.Doctor.MediumDoctors
                .Where(m => m.MedicalAttentionMedium != null)
                .Select(m => new LocationVM
            {
                Address = m.MedicalAttentionMedium.Place.Address,
                Agreements = m.InsuranceLocations
                .Select(i => i.InsuranceAgreement.Insurance.GetAttrName()),
                PhotoId = m.MedicalAttentionMedium.Place.PhotoId,
                CId = m.MedicalAttentionMedium.Place.CId,
                Name = m.MedicalAttentionMedium.Place.Name,
                Commune = m.MedicalAttentionMedium.Place.Commune.Name,
                PlaceId = m.MedicalAttentionMedium.PlaceId
            });
            return View(model);
        }
        public async Task<IActionResult> Agreements()
        {
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.MedicalAttentionMedium)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.InsuranceLocations)
                                .ThenInclude(i => i.Commune)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.InsuranceLocations)
                                .ThenInclude(i => i.InsuranceAgreement)
                                    .ThenInclude(i => i.InsuranceLocations)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.InsuranceLocations)
                                .ThenInclude(i => i.InsuranceAgreement)
                                    .ThenInclude(i => i.InsuranceLocations)
                                        .ThenInclude(i => i.Prestacion)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);
            var model = user.Person.Doctor.MediumDoctors
                .Where(m => m.InsuranceLocations != null)
                .SelectMany(i => i.InsuranceLocations.Select(l => l.InsuranceAgreement))
                .Distinct().Select(m => new AgreementVM
                {
                    Id = m.Id,
                    Insurance = m.Insurance,
                    PersonRUT = RUT.Format(m.PersonId),
                    PersonName = m.Person.GetShortName(),
                    ActiveLocations = m.InsuranceLocations
                    .Where(i => i.MediumDoctor.MedicalAttentionMedium != null)
                    .Select(i => i.GetName()),
                    InactiveLocations = m.InsuranceLocations
                    .Where(i => i.MediumDoctor.MedicalAttentionMedium == null)
                    .Select(i => i.GetName())
                });
            return View(model);
        }
    }
}