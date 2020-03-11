using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Areas.Patients.Views.Search;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM.PatientsVM;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients"), Authorize]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRedirect _redirect;
        public SearchController(
            ApplicationDbContext context,
            IRedirect redirect)
        {
            _redirect = redirect;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult GetImg(int id)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                "profileImg", id.ToString(CultureInfo.InvariantCulture) + ".min.jpg");
            var physical = System.IO.File.Exists(file) ? file 
                : Path.Combine(Directory.GetCurrentDirectory(), DefaultImageMiddleware.DefaultImagePath);
            return PhysicalFile(physical, "image/jpg");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id 
                ?? HttpContext.TraceIdentifier });
        }

        [HttpPost, ValidateAntiForgeryToken, ProducesResponseType(typeof(JsonResult), 
            StatusCodes.Status200OK)]
        public async Task<IActionResult> MapList(
            [Bind("Ubicacion,Especialidad,Sex,Insurance," +
            "HighlightInsurance,MinTime,MaxTime,Dates,Last")]
        MapVM filters
            //Medium Discriminator Office = 1, Home = 2, Remote = 3
            //string disc,
            )
        {
            if (filters == null) return NotFound();

            //var ma = await _context.MedicalOffices
            //.Include(o => o.Place)
            //    .ThenInclude(p => p.Commune)
            //        .ThenInclude(c => c.Province)
            //.Include(o => o.MediumDoctors)
            //    .ThenInclude(md => md.Doctor)
            //        .ThenInclude(d => d.Natural)
            //.Include(o => o.MediumDoctors)
            //    .ThenInclude(d => d.AgendaEvents)
            //    .ThenInclude(d => d.Agendas)
            //        .ThenInclude(at => at.TimeSlots)
            //.Include(o => o.MediumDoctors)
            //    .ThenInclude(o => o.InsuranceLocations)
            //        .ThenInclude(i => i.InsuranceAgreement)
            //.ToListAsync().ConfigureAwait(false);

            //ma.Where(o => !filters.Ubicacion.Any()
            //|| filters.Ubicacion.Any(f => f == o.Place.CommuneId 
            //|| f == o.Place.Commune.ProvinceId
            //|| f == o.Place.Commune.Province.RegionId)
            //);

            //var mo = ma
            //    .GroupBy(o => o.Place);

            //var final = new List<ResultsVM>();

            //foreach (var item in mo)
            //{
            //    var place = new PlaceVM
            //    {
            //        Address = item.Key.Address,
            //        CId = item.Key.CId,
            //        Id = item.Key.Id,
            //        Name = item.Key.Name,
            //        Latitude = item.Key.Latitude,
            //        Longitude = item.Key.Longitude,
            //        PhotoId = item.Key.PhotoId
            //    };

            //    var items = new List<ResultVM>();

            //    foreach (MedicalOffice office in item)
            //    {
            //        var mds = office.MediumDoctors
            //        .Where(md =>
            //        md.MedicalAttentionMediumId.HasValue
            //        && (!filters.Especialidad.Any()
            //        || filters.Especialidad.Any(e => md.Doctor.Specialties.Any(s => s.SpecialtyId == e)))
            //        && (!filters.Sex.Any()
            //        || filters.Sex.Contains(md.Doctor.Natural.Sex))
            //        && md.AgendaEvents.Any(e =>
            //        e.Agendas.Any(a => a.StartTime > DateTime.Now.AddMinutes(30)
                    //&& (!filters.Dates.Any()
                    //|| filters.Dates.Any(d => d.Date == a.StartTime.Date))
                    //&& a.TimeSlots.Any(t =>
                    //!t.ReservationId.HasValue
                    //&& a.StartTime > DateTime.Now.AddMinutes(30)
                    //&& (!filters.MinTime.HasValue
                    //|| filters.MinTime.Value <= t.StartTime.TimeOfDay)
                    //&& (!filters.MaxTime.HasValue
                    //|| filters.MaxTime.Value >= t.EndTime.TimeOfDay)
                    //)
            //        )
            //        )
            //        );

            //        var ul = new List<ResultVM>();

            //        foreach (MediumDoctor md in mds)
            //        {
            //            var r = new ResultVM
            //            {
            //                Run = md.Doctor.NaturalId,
            //                Dr = md.Doctor.Natural.FullNameFirst,
            //                Experience = md.Doctor.GetYearsExperience(),
            //                Especialidades = md.Doctor.Specialties.Select(s => s.Specialty.Name),
            //                Price = md.PriceParticular,
            //                Insurances = md.InsuranceLocations.Select(i => i.InsuranceAgreement.Insurance),
            //                Match = filters.Insurance == (int)Insurance.Particular
            //                    || !filters.HighlightInsurance
            //                    || md.InsuranceLocations.Any(i => 
            //                    (int)i.InsuranceAgreement.Insurance == filters.Insurance),
            //                Sex = md.Doctor.Natural.Sex,
            //                Office = string.Join(" ", (!string
            //                .IsNullOrEmpty(office.Appartment) ? "dpto." + office.Appartment : ""),
            //                    (!string.IsNullOrEmpty(office.Floor) ? "piso " + office.Floor : ""),
            //                    (!string.IsNullOrEmpty(office.Office) ? "of. " + office.Office : "")),
            //                NextTS = new TimeSlotVM(md.AgendaEvents.SelectMany(e => e.Agendas.Where(a =>
            //                a.StartTime > DateTime.Now.AddMinutes(30)
            //                && (!filters.Dates.Any()
            //                || filters.Dates.Any(d => d.Date == a.StartTime.Date))
            //                ))
            //                    .SelectMany(a => a.TimeSlots.Where(t =>
            //                    !t.ReservationId.HasValue
            //                    && a.StartTime > DateTime.Now.AddMinutes(30)
            //                    && (!filters.MinTime.HasValue
            //                    || filters.MinTime.Value <= t.StartTime.TimeOfDay)
            //                    && (!filters.MaxTime.HasValue
            //                    || filters.MaxTime.Value >= t.EndTime.TimeOfDay)
            //                        )).MinBy(t => t.StartTime).First()),
            //                CardId = md.Id
            //            };

            //            ul.Add(r);
            //        }

            //        items.AddRange(ul);
            //    }

            //    final.Add(new ResultsVM { Place = place, Items = items
            //        .OrderBy(res => res.NextTS.StartTime) });
            //}

            //var final2 = final.Where(rs => rs.Items.Any())
            //    .OrderBy(rs => rs.Items.Min(i => i.NextTS.StartTime));

            var tmp = await _context.MedicalOffices
                .Include(o => o.Place)
                    .ThenInclude(p => p.Commune)
                        .ThenInclude(c => c.Province)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(md => md.Doctor)
                        .ThenInclude(d => d.Natural)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(d => d.AgendaEvents)
                    .ThenInclude(d => d.Agendas)
                        .ThenInclude(at => at.TimeSlots)
                .Include(o => o.MediumDoctors)
                    .ThenInclude(o => o.InsuranceLocations)
                        .ThenInclude(i => i.InsuranceAgreement)
                                        .Where(o => !filters.Ubicacion.Any()
                || filters.Ubicacion.AsQueryable().Contains(o.Place.CommuneId)
                || filters.Ubicacion.AsQueryable().Contains(o.Place.Commune.ProvinceId)
                || filters.Ubicacion.AsQueryable().Contains(o.Place.Commune.Province.RegionId)
                )
                        .ToListAsync().ConfigureAwait(false);

            return Ok(tmp
                .GroupBy(o => o.Place)
                .Select(item => new ResultsVM
                {
                    Place = new PlaceVM
                    {
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
                            md.MedicalAttentionMediumId.HasValue
                            && (!filters.Especialidad.Any()
                            || filters.Especialidad.Any(e => md.Doctor.Specialties
                            .Any(s => s.SpecialtyId == e)))
                            && (!filters.Sex.Any()
                            || filters.Sex.AsQueryable().Contains(md.Doctor.Natural.Sex))
                            && md.AgendaEvents.Any(e => e.Agendas
                            .Any(a => a.StartTime > DateTime.Now.AddMinutes(30)
                            && (!filters.Dates.Any()
                            || filters.Dates.Any(d => d.Date == a.StartTime.Date))
                            && a.TimeSlots.Any(t =>
                            !t.ReservationId.HasValue
                            && a.StartTime > DateTime.Now.AddMinutes(30)
                            && (!filters.MinTime.HasValue
                            || filters.MinTime.Value <= t.StartTime.TimeOfDay)
                            && (!filters.MaxTime.HasValue
                            || filters.MaxTime.Value >= t.EndTime.TimeOfDay)
                            )
                            )
                            )
                            )
                            .Select(md =>
                            new ResultVM
                            {
                                Run = md.Doctor.NaturalId,
                                Dr = md.Doctor.Natural.FullNameFirst,
                                Experience = md.Doctor.GetYearsExperience(),
                                Especialidades = md.Doctor.Specialties.Select(s => s.Specialty.Name),
                                Price = md.PriceParticular,
                                Insurances = md.InsuranceLocations
                                .Select(i => i.InsuranceAgreement.Insurance),
                                Match = filters.Insurance == (int)Insurance.Particular
                                || !filters.HighlightInsurance
                                || md.InsuranceLocations
                                .Any(i => (int)i.InsuranceAgreement.Insurance == filters.Insurance),
                                Sex = md.Doctor.Natural.Sex,
                                Office = string
                                .Join(" ", (!string
                                .IsNullOrEmpty(office.Appartment) ? "dpto." + office.Appartment : ""),
                                (!string.IsNullOrEmpty(office.Floor) ? "piso " + office.Floor : ""),
                                (!string.IsNullOrEmpty(office.Office) ? "of. " + office.Office : "")),
                                NextTS = new TimeSlotVM(md.AgendaEvents
                                .SelectMany(e => e.Agendas.Where(a =>
                                a.StartTime > DateTime.Now.AddMinutes(30)
                                && (!filters.Dates.Any()
                                || filters.Dates.Any(d => d.Date == a.StartTime.Date))
                                ))
                                .SelectMany(a => a.TimeSlots.Where(t =>
                                !t.ReservationId.HasValue
                                && a.StartTime > DateTime.Now.AddMinutes(30)
                                && (!filters.MinTime.HasValue
                                || filters.MinTime.Value <= t.StartTime.TimeOfDay)
                                && (!filters.MaxTime.HasValue
                                || filters.MaxTime.Value >= t.EndTime.TimeOfDay)
                                    )).MinBy(t => t.StartTime).First()),
                                CardId = md.Id
                            }))
                    .OrderBy(res => res.NextTS.StartTime)
                }).Where(rs => rs.Items.Any()).OrderBy(rs => rs.Items.Min(i => i.NextTS.StartTime)));
        }
        [HttpPost, ValidateAntiForgeryToken, 
            ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult FilterLists()
        {
            var comunas = _context.Communes
                .Where(c => c.Places.Any())
                .Include(c => c.Province)
                .ThenInclude(p => p.Region);

            return Json(new
            {
                esp = _context.Specialties
                .Select(s => new {
                    id = s.Id,
                    text = s.Name,
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
            var page = await _redirect.Init(User.Identity.Name).ConfigureAwait(false);
            if(page == "Done")
            {
                var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Patient)
                .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name)
                .ConfigureAwait(false);
                var model = new MapVM
                {
                    Insurance = (int)user.Person.Patient.Insurance,
                    Last = _context.TimeSlots.Max(t => t.StartTime),
                    MinTime = new TimeSpan(0,0,0),
                    MaxTime = new TimeSpan(23,59,59)
                };
                return View(model);
            }
            return LocalRedirect($"/Identity/Account/{page}?returnUrl=/Patients/Search/Map");
        }
        [HttpPost, ValidateAntiForgeryToken, 
            ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult MdData(int id)
        {
            return Json(_context.MediumDoctors
                .Include(m => m.InsuranceLocations)
                    .ThenInclude(i => i.InsuranceAgreement)
                .Include(m => m.AgendaEvents)
                    .ThenInclude(e => e.Agendas)
                .Where(m => m.DoctorId == id
                && m.AgendaEvents.Any(e => e.Agendas.Any(a =>
                a.StartTime > DateTime.Now.AddMinutes(30)
                && a.TimeSlots.Any(ts => !ts.ReservationId.HasValue
                && ts.StartTime > DateTime.Now.AddMinutes(30)))))
                .ToDictionary(m => m.Id,
                m => new Dictionary<int, string[]>
                {
                { 0, m.InsuranceLocations
            .Select(i => i.InsuranceAgreement.Insurance.GetAttrName()).ToArray() },
                { 1, m.AgendaEvents.SelectMany(e => e.Agendas)
            .DistinctBy(a => a.StartTime.Date)
            .Select(a => a.StartTime.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)).ToArray() }
                }));
        }
        [HttpPost, ValidateAntiForgeryToken, 
            ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public JsonResult TimeSlots(DateTime startDate, int mdId)
        {
            return Json(_context.TimeSlots
                .Include(ts => ts.Agenda)
                .Where(ts => ts.Agenda.AgendaEvent.MediumDoctorId == mdId
                && ts.StartTime.Date == startDate.Date
                && !ts.ReservationId.HasValue)
                .Select(ts => new TimeSlotsVM
                {
                    Id = ts.Id,
                    StartTime = ts.StartTime.ToString("HH:mm tt", new CultureInfo("es-CL"))
                }));
        }
        [HttpPost, ValidateAntiForgeryToken, 
            ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public JsonResult GetDates(int mdId, DateTime? minDate = null, DateTime? maxDate = null)
        {
            return Json(_context.Agenda
                .Where(a => a.AgendaEvent.MediumDoctorId == mdId
                && a.StartTime > DateTime.Now.AddMinutes(30)
                && (!minDate.HasValue || minDate.Value <= a.StartTime)
                && (!maxDate.HasValue || maxDate.Value >= a.EndTime)
                && a.TimeSlots.Any(ts => !ts.ReservationId.HasValue))
                .Select(ts => int.Parse(ts.StartTime
                .ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture)).Distinct());
        }
        public async Task<IActionResult> DoctorDetails([Bind("Ubicacion,Insurance," +
            "MinTime,MaxTime,MinDate,MaxDate," +
            "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Days,Last")]
        MapVM filters, int id, int MdId)
        {
            var doc = await _context.Doctors
                .Include(d => d.Natural)
                .SingleOrDefaultAsync(d => d.NaturalId == id).ConfigureAwait(false);
            var title = $"Dr{(doc.Natural.Sex ? ". " : "a. ")}";
            ViewData["Title"] = $"<span class=\"hide-on-small-only\">Detalles</span> {title}{doc.Natural.GetShortName()}";

            var mds = _context.MediumDoctors
                .Include(md => md.AgendaEvents)
                    .ThenInclude(e => e.Agendas)
                .Include(md => md.MedicalAttentionMedium)
                    .ThenInclude(mo => mo.Place)
                        .ThenInclude(p => p.Commune)
                .Where(md => md.DoctorId == doc.Id
                && md.AgendaEvents.Any(e => e.Agendas
                .Any(a => a.TimeSlots.Any(ts => !ts.ReservationId.HasValue))));

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
            ViewData["Title"] = $"<span class=\"hide-on-small-only\">Detalles</span> {model?.Title}{doc.Natural.GetShortName()}";

            var mds = _context.MediumDoctors
                .Include(md => md.AgendaEvents)
                .ThenInclude(md => md.Agendas)
                .Include(md => md.MedicalAttentionMedium)
                    .ThenInclude(mo => mo.Place)
                        .ThenInclude(p => p.Commune)
                .Where(md => md.DoctorId == doc.Id
                && md.AgendaEvents.Any(e => e.Agendas.Any(a => a.TimeSlots.Any(ts => !ts.ReservationId.HasValue))));

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
    public class ResultsVM
    {
        public PlaceVM Place { get; set; }
        public IOrderedEnumerable<ResultVM> Items { get; set; }
    }
    public class ResultVM
    {
        public int Run { get; set; }
        public int Price { get; set; }
        public string Dr { get; set; }
        public string Office { get; set; }
        public IEnumerable<string> Especialidades { get; set; }
        public int Experience { get; set; }
        public bool Sex { get; set; }
        public IEnumerable<Insurance> Insurances { get; set; }
        public TimeSlotVM NextTS { get; set; }
        public int CardId { get; set; }
        public bool Match { get; set; }
    }
}