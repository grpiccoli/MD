using ConsultaMD.Areas.MDs.Models;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.MDs.Controllers
{
    [Area("MDs")]
    public class MDashController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MDashController(ApplicationDbContext context)
        {
            _context = context;
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
                .Where(t => t.Agenda.AgendaEvent.MediumDoctor.Doctor.Natural.ApplicationUser.UserName == User.Identity.Name);
            var model = new SummaryVM
            {
                TimeSlots = timeSlots.Count(),
                TakenTimeSlots = timeSlots.Count(t => t.Reservation != null)
            };
            return View(model);
        }
        public IActionResult MDWaitingRoom()
        {
            return View();
        }
        public async Task<IActionResult> Agenda()
        {
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.MedicalAttentionMedium)
                                .ThenInclude(m => m.Place)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.MediumDoctors)
                            .ThenInclude(m => m.AgendaEvents)
                                .ThenInclude(m => m.Agendas)
                                    .ThenInclude(m => m.TimeSlots)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);
            var model = user.Person.Doctor.MediumDoctors.SelectMany(m => m.AgendaEvents.Select(a => new AgendaEventVM
            {
                Location = m.MedicalAttentionMedium.Place.Name,
                StartTime = a.StartDateTime.TimeOfDay,
                EndTime = a.EndDateTime.TimeOfDay,
                Duration = a.Duration,
                TimeSlots = a.Agendas.SelectMany(ag => ag.TimeSlots)
            }));
            return View(model);
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
                Agreements = m.InsuranceLocations.Select(i => i.InsuranceAgreement.Insurance.GetAttrName()),
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