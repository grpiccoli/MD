using System.Collections.Generic;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult MapList(
            [Bind("Ubicacion,Especialidad,Sex,Insurance," +
            "MinTime,MaxTime,MinDate,MaxDate," +
            "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Days")]
        MapVM filters
            //Medium Discriminator Office = 1, Home = 2, Remote = 3
            //string disc,
            )
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
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
                            && (!filters.Sex.Any() || filters.Sex.Contains(md.Doctor.Sex))
                            && md.Agendas.Any(a =>
                                   (!filters.Days.Any() || !filters.Days.Contains(a.StartTime.DayOfWeek))
                               && a.TimeSlots.Any(t => !t.Taken
                                && (!filters.MinTime.HasValue || filters.MinTime.Value <= t.StartTime.Hour)
                                && (!filters.MaxTime.HasValue || filters.MaxTime.Value >= t.EndTime.Hour)
                                && (!filters.MinDate.HasValue || filters.MinDate.Value <= t.StartTime)
                                && (!filters.MaxDate.HasValue || filters.MaxDate.Value >= t.EndTime))))
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
                                || md.InsuranceLocations.Select(i => i.Insurance).Contains(filters.Insurance),
                                Sex = md.Doctor.Sex,
                                Office = string.Join(" ",(!string.IsNullOrEmpty(office.Appartment)?"dpto."+office.Appartment:""),
                                (!string.IsNullOrEmpty(office.Floor)?"piso "+office.Floor:""),
                                (!string.IsNullOrEmpty(office.Office)?"of. "+office.Office:"")),
                                Next = md.Agendas.Where(a =>
                                    !filters.Days.Any() || !filters.Days.Contains(a.StartTime.DayOfWeek))
                                    .SelectMany(a => a.TimeSlots.Where(t => !t.Taken
                                      && (!filters.MinTime.HasValue || filters.MinTime.Value <= t.StartTime.Hour)
                                      && (!filters.MaxTime.HasValue || filters.MaxTime.Value >= t.EndTime.Hour)
                                      && (!filters.MinDate.HasValue || filters.MinDate.Value <= t.StartTime)
                                      && (!filters.MaxDate.HasValue || filters.MaxDate.Value >= t.EndTime)
                                    )).Min(t => t.StartTime)
                            })).Where(res => res.Next.HasValue)
                            .OrderBy(res => res.Next)
                }).Where(rs => rs.Items.Any()).OrderBy(rs => rs.Items.Min(i => i.Next));
            watch.Stop();
            return Ok(result);
        }
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public JsonResult NameList()
        {
            var model = _context.Doctors
                .Include(d => d.Natural)
                .Select(d => new
                    {
                        name = d.Natural.FullNameFirst, 
                        img = $"/img/doc/{d.NaturalId}.min.jpg"
                    })
                .ToDictionary(d => d.name, d => d.img);
            return Json(model);
        }
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult FilterLists()
        {
            var comunas = _context.Communes
                .Where(c => c.Places.Any())
                .Include(c => c.Province)
                .ThenInclude(p => p.Region);

            var comlist = comunas
                .Select(g =>
                new
                {
                    id = g.Id,
                    text = g.Name
                });

            var provlist = comunas
                .Select(c =>
                new
                {
                    id = c.ProvinceId,
                    text = c.Province.Name
                }).Distinct();

            var reglist = comunas
                .Select(c =>
                new
                {
                    id = c.Province.RegionId,
                    text = c.Province.Region.Name
                }).Distinct();

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
                            children = reglist,
                        },
                        new
                        {
                            id = 2,
                            text = "Provincias",
                            children = provlist,
                        },
                        new
                        {
                            id = 3,
                            text = "Comunas",
                            children = comlist,
                        }
                    }
                });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        public IActionResult InsuranceList(int id)
        {
            return Json(_context.MedicalOffices
                .Include(o => o.MediumDoctors)
                    .ThenInclude(m => m.InsuranceLocations)
                .Include(o => o.Place)
                    .ThenInclude(p => p.Commune)
                .Where(o => o.MediumDoctors.Any(m => m.DoctorId == id))
                .ToDictionary(m =>
                    m.PlaceId, m => m.MediumDoctors
                    .SelectMany(d => d.InsuranceLocations
                    .Select(i => i.Insurance.GetAttrName())).ToArray()));
        }
        public async Task<IActionResult> Map()
        {
            ViewData["Title"] = "Buscar Cita";
            ViewData["run"] = User.Identity.Name.Replace(".", "").Split("-")[0];
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.NaturalId == user.PersonId);

            var model = new MapVM();
            if (patient != null)
            {
                if (user.PhoneNumberConfirmed)
                {
                    model.Insurance = patient.Insurance;
                    model.Last = _context.TimeSlots.Max(t => t.StartTime);
                    return View(model);
                }
                return LocalRedirect("/Identity/Account/VerifyPhone?returnUrl=/Search/Map");
            }
            return LocalRedirect("/Identity/Account/InsuranceDetails?returnUrl=/Search/Map");
        }
        public async Task<IActionResult> DoctorDetails([Bind("Ubicacion,Insurance," +
            "MinTime,MaxTime,MinDate,MaxDate," +
            "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Days,Last")]
        MapVM filters, int id, string placeId)
        {
            ViewData["run"] = User.Identity.Name.Replace(".", "").Split("-")[0];
            var doc = await _context.Doctors
                .Include(d => d.Natural)
                .SingleOrDefaultAsync(d => d.NaturalId == id);
            var title = doc.Specialty.HasValue ? $"Dr{(doc.Sex ? ". " : "a. ")}" : "";
            ViewData["Title"] = $"Detalles {title}{doc.Natural.FullNameFirst}";
            var mo = _context.MedicalOffices
                .Include(o => o.MediumDoctors)
                    .ThenInclude(m => m.InsuranceLocations)
                .Include(o => o.Place)
                    .ThenInclude(p => p.Commune)
                .Where(o => o.MediumDoctors.Any(m => m.DoctorId == doc.Id));
            var places = mo.Select(m => new SelectListItem
                {
                    Value = m.PlaceId,
                    Text = $"{m.Place.Name}, {m.Place.Commune.Name}",
                    Selected = placeId == m.PlaceId
                });
            var model = new DoctorDetailsVM
            {
                DocVM = doc,
                PlaceId = placeId,
                Title = title,
                PlacesList = places,
                Last = filters.Last
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorDetails([Bind("DocVM, PlaceId, Date, Time")] DoctorDetailsVM model)
        {
            ViewData["run"] = User.Identity.Name.Replace(".", "").Split("-")[0];
            if (ModelState.IsValid)
            {
                int.TryParse(User.Identity.Name.Replace(".", "").Split("-")[0], out var run);
                var patient = _context.Patients.SingleOrDefault(p => p.NaturalId == run);
                var doctor = _context.Doctors.SingleOrDefault(d => d.NaturalId == model.DocVM.NaturalId);
                var place = _context.Places.SingleOrDefault(p => p.Id == model.PlaceId);
                var paymentvm = new PaymentVM
                {
                    RutCliente = User.Identity.Name,
                    Dia = model.Date.ToString("yyyy-MM-dd"),
                    Hora = model.Time.ToString("HH:mm"),
                    Especialidad = doctor.Specialty.GetAttrName(),
                    EspecialidadId = doctor.Specialty,
                    RutDoctor = model.DocVM.Natural.GetRUT(),
                    Direccion = place.Address,
                    NombreDoctor = doctor.Natural.FullNameFirst,
                    NombreCliente = patient.Natural.FullNameFirst,
                    DoctorId = model.DocVM.NaturalId,
                    area = "Patients",
                    Lugar = place.Name,
                    PlaceId = place.Id
                };
                return RedirectToAction("ConfirmPayment", "Booking", paymentvm);
            }
            model.DocVM = await _context.Doctors
                .SingleOrDefaultAsync(d => d.NaturalId == model.DocVM.NaturalId);
            model.PlacesList = _context.MedicalOffices
                .Include(o => o.MediumDoctors)
                .Include(d => d.Place)
                    .ThenInclude(p => p.Commune)
                .Where(m => m.MediumDoctors.Any(d => d.DoctorId == model.DocVM.Id))
                .Select(m => new SelectListItem
                {
                    Value = m.PlaceId,
                    Text = $"{m.Place.Name}, {m.Place.Commune.Name}"
                });

            return View(model);
        }

    }
}