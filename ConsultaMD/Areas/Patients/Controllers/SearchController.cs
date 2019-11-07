using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Areas.Patients.Views.Search;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM.PatientsVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients"), Authorize]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult MapList(
            [Bind("Ubicacion,Especialidad,Sex,Insurance," +
            "MinTime,MaxTime,Dates")]
        MapVM filters
            //Medium Discriminator Office = 1, Home = 2, Remote = 3
            //string disc,
            )
        {
            var result = _context.MedicalOffices
                .Include(o => o.Place)
                    .ThenInclude(p => p.Commune)
                        .ThenInclude(c => c.Province)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(md => md.Doctor)
                        .ThenInclude(d => d.Natural)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(d => d.Agendas)
                        .ThenInclude(at => at.TimeSlots)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(o => o.InsuranceLocations)
                .Where(o => !filters.Ubicacion.Any()
                || filters.Ubicacion.Contains(o.Place.CommuneId) 
                || filters.Ubicacion.Contains(o.Place.Commune.ProvinceId)
                || filters.Ubicacion.Contains(o.Place.Commune.Province.RegionId)
                )
                .GroupBy(o => o.Place)
                .Select(item => new ResultsVM
                {
                    Place = new PlaceVM { 
                        Address = item.Key.Address,
                        CId = item.Key.CId,
                        Id = item.Key.Id,
                        Name = item.Key.Name,
                        Latitude = item.Key.Latitude,
                        Longitude = item.Key.Longitude,
                        PhotoId = item.Key.PhotoId
                    },
                    Items = item.SelectMany(office =>
                            office.MediumDoctors
                            .Where(md =>
                            (!filters.Especialidad.Any() || (md.Doctor.Specialty.HasValue
                            && filters.Especialidad.Contains(md.Doctor.Specialty.Value)))
                            && (!filters.Sex.Any() || filters.Sex.Contains(md.Doctor.Natural.Sex))
                            && md.Agendas.Any(a => a.StartTime > DateTime.Now.AddMinutes(30)
                            && (!filters.Dates.Any() || filters.Dates.Any(d => d.Date == a.StartTime.Date))
                                   //&& (!filters.Days.Any() || !filters.Days.Contains(a.StartTime.DayOfWeek))
                               && a.TimeSlots.Any(t => !t.ReservationId.HasValue
                               && a.StartTime > DateTime.Now.AddMinutes(30)
                                && (!filters.MinTime.HasValue || filters.MinTime.Value <= t.StartTime.TimeOfDay)
                                && (!filters.MaxTime.HasValue || filters.MaxTime.Value >= t.EndTime.TimeOfDay))))
                                //&& (!filters.MinDate.HasValue || (filters.MinDate.Value.TimeOfDay <= t.StartTime.TimeOfDay && filters.MinDate.Value <= t.StartTime))
                                //&& (!filters.MaxDate.HasValue || (filters.MaxDate.Value.TimeOfDay >= t.EndTime.TimeOfDay && filters.MaxDate.Value >= t.EndTime)))))
                            .Select(md =>
                            new ResultVM
                            {
                                Run = md.Doctor.NaturalId,
                                Dr = md.Doctor.Natural.FullNameFirst,
                                Experience = md.Doctor.GetYearsExperience(),
                                Especialidad = md.Doctor.Specialty == null ? null : md.Doctor.Specialty.GetAttrName(),
                                Price = md.PriceParticular,
                                Insurances = md.InsuranceLocations.Select(i => i.Insurance),
                                Match = filters.Insurance == InsuranceData.Insurance.Particular 
                                || !filters.HighlightInsurance 
                                || md.InsuranceLocations.Any(i => i.Insurance == filters.Insurance),
                                Sex = md.Doctor.Natural.Sex,
                                Office = string.Join(" ",(!string.IsNullOrEmpty(office.Appartment)?"dpto."+office.Appartment:""),
                                (!string.IsNullOrEmpty(office.Floor)?"piso "+office.Floor:""),
                                (!string.IsNullOrEmpty(office.Office)?"of. "+office.Office:"")),
                                NextTS = new TimeSlotVM(md.Agendas.Where(a => a.StartTime > DateTime.Now.AddMinutes(30)
                                    && (!filters.Dates.Any() || filters.Dates.Any(d => d.Date == a.StartTime.Date))
                                    //&& (!filters.Days.Any() || !filters.Days.Contains(a.StartTime.DayOfWeek))
                                    )
                                    .SelectMany(a => a.TimeSlots.Where(t => !t.ReservationId.HasValue
                                    && a.StartTime > DateTime.Now.AddMinutes(30)
                                && (!filters.MinTime.HasValue || filters.MinTime.Value <= t.StartTime.TimeOfDay)
                                && (!filters.MaxTime.HasValue || filters.MaxTime.Value >= t.EndTime.TimeOfDay)
                                    )).MinBy(t => t.StartTime).First()),
                                CardId = md.Id
                            })).OrderBy(res => res.NextTS.StartTime)
                }).Where(rs => rs.Items.Any()).OrderBy(rs => rs.Items.Min(i => i.NextTS.StartTime));
            return Ok(result);
        }
        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult FilterLists()
        {
            var comunas = _context.Communes
                .Where(c => c.Places.Any())
                .Include(c => c.Province)
                .ThenInclude(p => p.Region);

            return Json(new
            {
                esp = _context.MediumDoctors
                .Include(c => c.Doctor)
                .Where(md => md.Doctor.Specialty.HasValue)
                .Select(md => new {
                    id = (int)md.Doctor.Specialty.Value,
                    text = md.Doctor.Specialty.GetAttrName(),
                }).Distinct(),
                    loc = new object[]
                    {
                        new
                        {
                            id = 1,
                            text = "Regiones",
                            children = comunas.Select(c => new
                            {
                                id = c.Province.RegionId,
                                text = c.Province.Region.Name
                            }).Distinct(),
                        },
                        new
                        {
                            id = 2,
                            text = "Provincias",
                            children = comunas.Select(c => new
                            {
                                id = c.ProvinceId,
                                text = c.Province.Name
                            }).Distinct(),
                        },
                        new
                        {
                            id = 3,
                            text = "Comunas",
                            children = comunas.Select(g => new
                            {
                                id = g.Id,
                                text = g.Name
                            }),
                        }
                    }
                });
        }
        public async Task<IActionResult> Map()
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name).ConfigureAwait(false);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.NaturalId == user.PersonId).ConfigureAwait(false);

            var model = new MapVM();
            if (patient != null)
            {
                if (user.PhoneNumberConfirmed)
                {
                    model.Insurance = patient.Insurance;
                    model.Last = _context.TimeSlots.Max(t => t.StartTime);
                    return View(model);
                }
                return LocalRedirect("/Identity/Account/VerifyPhone?returnUrl=/Patients/Search/Map");
            }
            return LocalRedirect("/Identity/Account/InsuranceDetails?returnUrl=/Patients/Search/Map");
        }
        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult MdData(int id)
        {
            return Json(_context.MediumDoctors
                .Include(m => m.InsuranceLocations)
                .Include(m => m.Agendas)
                .Where(m => m.DoctorId == id
                && m.Agendas.Any(a =>
                a.StartTime > DateTime.Now.AddMinutes(30)
                && a.TimeSlots.Any(ts => !ts.ReservationId.HasValue
                && ts.StartTime > DateTime.Now.AddMinutes(30))))
                .ToDictionary(m => m.Id,
                m => new Dictionary<int, string[]>
                {
                { 0, m.InsuranceLocations
            .Select(i => i.Insurance.GetAttrName()).ToArray() },
                { 1, m.Agendas
            .DistinctBy(a => a.StartTime.Date)
            .Select(a => a.StartTime.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)).ToArray() }
                }));
        }
        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public JsonResult TimeSlots(DateTime startDate, int mdId)
        {
            return Json(_context.TimeSlots
                .Include(ts => ts.Agenda)
                .Where(ts => ts.Agenda.MediumDoctorId == mdId
                && ts.StartTime.Date == startDate.Date
                && !ts.ReservationId.HasValue)
                .Select(ts => new TimeSlotsVM
                {
                    Id = ts.Id,
                    StartTime = ts.StartTime.ToString("HH:mm tt", new CultureInfo("es-CL"))
                }));
        }
        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public JsonResult GetDates(int mdId, DateTime? minDate = null, DateTime? maxDate = null)
        {
            return Json(_context.Agenda
                .Where(a => a.MediumDoctorId == mdId
                && a.StartTime > DateTime.Now.AddMinutes(30)
                && (!minDate.HasValue || minDate.Value <= a.StartTime)
                && (!maxDate.HasValue || maxDate.Value >= a.EndTime)
                && a.TimeSlots.Any(ts => !ts.ReservationId.HasValue))
                .Select(ts => int.Parse(ts.StartTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture),CultureInfo.InvariantCulture)).Distinct());
        }
        public async Task<IActionResult> DoctorDetails([Bind("Ubicacion,Insurance," +
            "MinTime,MaxTime,MinDate,MaxDate," +
            "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Days,Last")]
        MapVM filters, int id, int MdId)
        {
            var doc = await _context.Doctors
                .Include(d => d.Natural)
                .SingleOrDefaultAsync(d => d.NaturalId == id).ConfigureAwait(false);
            var title = doc.Specialty.HasValue ? $"Dr{(doc.Natural.Sex ? ". " : "a. ")}" : "";
            ViewData["Title"] = $"<span class=\"hide-on-small-only\">Detalles</span> {title}{doc.Natural.GetName()} {doc.Natural.GetSurname()}";

            var mds = _context.MediumDoctors
                .Include(md => md.Agendas)
                .Include(md => md.MedicalAttentionMedium)
                    .ThenInclude(mo => mo.Place)
                        .ThenInclude(p => p.Commune)
                .Where(md => md.DoctorId == doc.Id
                && md.Agendas.Any(a => a.TimeSlots.Any(ts => !ts.ReservationId.HasValue)));

            var model = new DoctorDetailsVM
            {
                DocVM = doc,
                MdId = MdId,
                MdList = mds.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(CultureInfo.InvariantCulture),
                    Text = string.Join(", ", new List<string>{
                        m.MedicalAttentionMedium.Place.Name,
                        m.MedicalAttentionMedium.Place.Commune.Name,
                        (!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Appartment) ? ", dpto." + ((MedicalOffice)m.MedicalAttentionMedium).Appartment : null),
                        (!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Floor) ? "piso " + ((MedicalOffice)m.MedicalAttentionMedium).Floor : null),
                        (!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Office) ? "of. " + ((MedicalOffice)m.MedicalAttentionMedium).Office : null)
                    }.Where(i => !string.IsNullOrWhiteSpace(i))),
                    Selected = MdId == m.Id
                }),
                PlaceList = mds.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(CultureInfo.InvariantCulture),
                    Text = m.MedicalAttentionMedium.PlaceId
                }),
                Title = title,
                Last = filters?.Last
            };
            return View(model);
        }
        [HttpPost, HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservation(int id)
        {
            var rutParsed = RUT.Unformat(User.Identity.Name);
            var timeSlot = await _context.TimeSlots
                .SingleOrDefaultAsync(ts => ts.Id == id).ConfigureAwait(false);
            var reservation = new Reservation
            {
                PatientId = rutParsed.Value.rut,
                TimeSlotId = id
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var savedReserve = await _context.Reservations.SingleOrDefaultAsync(r => r.TimeSlotId == timeSlot.Id).ConfigureAwait(false);
            timeSlot.ReservationId = savedReserve.Id;
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            if (HttpContext.Request.Method == "POST")
            {
                return Json(Url.Action("ConfirmDate", "Booking", new { area = "Patients", id = reservation.Id }, Request.Scheme));
            }
            else
            {
                return RedirectToAction("ConfirmDate", "Booking", new { area = "Patients", id = reservation.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorDetails([Bind("TimeSlotId,DocVM,MdId,Last,Title,Date,Specialty")] DoctorDetailsVM model)
        {
            if (ModelState.IsValid && model != null)
            {
                var rutParsed = RUT.Unformat(User.Identity.Name);
                if (rutParsed.HasValue)
                {
                    var timeSlot = await _context.TimeSlots
                        .SingleOrDefaultAsync(ts => ts.Id == model.TimeSlotId).ConfigureAwait(false);
                    if(timeSlot != null)
                    {
                        return RedirectToAction("Reservation", new { id = timeSlot.Id });
                    }
                }
            }
            var doc = await _context.Doctors
                .Include(d => d.Natural)
                .SingleOrDefaultAsync(d => d.NaturalId == model.DocVM.NaturalId).ConfigureAwait(false);
            ViewData["Title"] = $"<span class=\"hide-on-small-only\">Detalles</span> {model?.Title}{doc.Natural.GetName()} {doc.Natural.GetSurname()}";

            var mds = _context.MediumDoctors
                .Include(md => md.Agendas)
                .Include(md => md.MedicalAttentionMedium)
                    .ThenInclude(mo => mo.Place)
                        .ThenInclude(p => p.Commune)
                .Where(md => md.DoctorId == doc.Id
                && md.Agendas.Any(a => a.TimeSlots.Any(ts => !ts.ReservationId.HasValue)));

            model = new DoctorDetailsVM
            {
                DocVM = doc,
                MdList = mds.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(CultureInfo.InvariantCulture),
                    Text =
$"{m.MedicalAttentionMedium.Place.Name}, {m.MedicalAttentionMedium.Place.Commune.Name}{(!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Appartment) ? ", dpto." + ((MedicalOffice)m.MedicalAttentionMedium).Appartment : "")}{(!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Floor) ? "piso " + ((MedicalOffice)m.MedicalAttentionMedium).Floor : "")}{(!string.IsNullOrEmpty(((MedicalOffice)m.MedicalAttentionMedium).Office) ? "of. " + ((MedicalOffice)m.MedicalAttentionMedium).Office : "")}",
                    Selected = model.MdId == m.Id
                }),
                PlaceList = mds.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(CultureInfo.InvariantCulture),
                    Text = m.MedicalAttentionMedium.PlaceId
                })
            };
            return View(model);
        }
    }
}