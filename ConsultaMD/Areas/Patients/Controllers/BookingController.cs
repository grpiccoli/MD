using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.VM;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients"), Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFonasa _fonasa;
        private readonly IFlow _flow;
        public BookingController(ApplicationDbContext context,
            IFlow flow,
            IFonasa fonasa)
        {
            _flow = flow;
            _fonasa = fonasa;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmDate(int id)
        {
            ViewData["referer"] = Request.Headers["Referer"].ToString();
            if (ModelState.IsValid)
            {
                var rutParsed = RUT.Unformat(User.Identity.Name);
                if (rutParsed != null)
                {
                    var reservation = await _context.Reservations
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(md => md.Doctor)
                                            .ThenInclude(d => d.Specialties)
                                                .ThenInclude(d => d.Specialty)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(a => a.InsuranceLocations)
                                            .ThenInclude(a => a.InsuranceAgreement)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(md => md.Doctor)
                                            .ThenInclude(d => d.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(md => md.MedicalAttentionMedium)
                                            .ThenInclude(mo => mo.Place)
                        .SingleOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);
                        var paymentvm = new ReservationDetails(reservation);
                        return View(paymentvm);
                }
            } 
            return RedirectToAction("Map", "Search", new { area = "Patients" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(string Rut, int Id)
        {
            if (ModelState.IsValid)
            {
                var rutParsed = RUT.Unformat(User.Identity.Name);
                if (rutParsed != null)
                {
                    var user = await _context.Users
                        .Include(u => u.Person)
                            .ThenInclude(p => p.Patient)
                        .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name).ConfigureAwait(false);
                    if(user != null)
                    {
                        var reservation = await _context.Reservations
                            .Include(r => r.Patient)
                                .ThenInclude(p => p.Natural)
                            .Include(r => r.TimeSlot)
                                .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(md => md.Doctor)
                                            .ThenInclude(d => d.Natural)
                            .Include(r => r.TimeSlot)
                                .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(m => m.InsuranceLocations)
                                            .ThenInclude(i => i.InsuranceAgreement)
                            .Include(r => r.TimeSlot)
                                .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                    .ThenInclude(a => a.MediumDoctor)
                                        .ThenInclude(md => md.MedicalAttentionMedium)
                                            .ThenInclude(mo => mo.Place)
                                                .ThenInclude(p => p.Commune)
                                                    .ThenInclude(c => c.Province)
                                                        .ThenInclude(p => p.Region)
                            .SingleOrDefaultAsync(r => r.Id == Id).ConfigureAwait(false);
                        if(reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.InsuranceLocations
                            .Any(i => i.InsuranceAgreement.Insurance == user.Person.Patient.Insurance))
                        {
                            switch (user.Person.Patient.Insurance)
                            {
                                case Insurance.Fonasa:
                                    var insuranceLocation = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.InsuranceLocations
                                        .SingleOrDefault(i => i.InsuranceAgreement.Insurance == Insurance.Fonasa);
                                    var docPrestador = await _context.Naturals.FindAsync(insuranceLocation.InsuranceAgreement.PersonId).ConfigureAwait(false);
                                    var centro = await _context.Companies.FindAsync(insuranceLocation.InsuranceAgreement.PersonId).ConfigureAwait(false);
                                    var nombre = string.Empty;
                                    if(centro != null)
                                    {
                                        nombre = centro.RazonSocial;
                                    }else if(docPrestador != null)
                                    {
                                        nombre = docPrestador.FullNameFirst;
                                    }
                                    var paymentData = new PaymentData
                                    {
                                        Commune = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Commune.GetCUT(),
                                        Region = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Commune.Province.Region.GetCUT(),
                                        CodEspeciali = int.Parse(insuranceLocation.InsuranceSelector, CultureInfo.InvariantCulture),
                                        PrestacionId = insuranceLocation.PrestacionId,
                                        RutTratante = RUT.Fonasa(reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.Doctor.NaturalId),
                                        RutPrestador = RUT.Fonasa(insuranceLocation.InsuranceAgreement.PersonId),
                                        NomTratante = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.Doctor.Natural.Names,
                                        NomPrestador = nombre,
                                        EmailNotification = user.Email,
                                        PayRut = RUT.Fonasa(Rut),
                                        CelularNotification = user.PhoneNumber.Replace("+56", "", System.StringComparison.InvariantCulture)
                                        .Replace(" ", "", System.StringComparison.InvariantCultureIgnoreCase),
                                        RutBeneficiario = RUT.Fonasa(user.PersonId),
                                        DataBenef = new DataBenef
                                        {
                                            Estado = 0,
                                            ExtApellidoPat = user.Person.LastFather,
                                            ExtApellidoMat = user.Person.LastMother,
                                            ExtNombres = user.Person.Names,
                                            ExtSexo = user.Person.Sex ? "M" : "F",
                                            ExtFechaNacimi = user.Person.Birth.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                            ExtNomCotizante = user.Person.FullNameFirst,
                                            ExtGrupoIng = user.Person.Patient.Tramo.Value.ToString(),
                                            FechaNacimi = user.Person.Birth.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                                            EdadBeneficiario = user.Person.Age()
                                        }
                                        //reservation.TimeSlot.Agenda.MediumDoctor.Doctor.Specialty.Value.ToString("d")
                                    };
                                    if (reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.Doctor.FonasaLevel.HasValue)
                                        paymentData.NivelPrestador = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.Doctor.FonasaLevel.Value;
                                    var fonasaWebPay = await _fonasa.PayAsync(paymentData).ConfigureAwait(false);
                                    return Ok(fonasaWebPay.Datos.Token);
                            }
                        }
                        else
                        {
                            //PAGO PARTICULAR
                            var url = _flow.PaymentCreate(
                                reservation.Id, "PagoParticular",
                                reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.PriceParticular,
                                user.Email);
                            return Ok(url);
                        }
                    }
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> Appointments()
        {
            var rut = RUT.Unformat(User.Identity.Name);
            var reservations = await _context.Reservations
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => md.Doctor)
                                        .ThenInclude(d => d.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.AgendaEvent)
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => md.MedicalAttentionMedium)
                                        .ThenInclude(mo => mo.Place)
                .Where(r => r.PatientId == rut.Value.rut).Select(r => new ReservationDetails(r))
                .ToListAsync().ConfigureAwait(false);
            return View(reservations);
        }

        public IActionResult CancelBookingDoctor()
        {
            return View();
        }
        public IActionResult TransactionHistory()
        {
            return View();
        }
    }
}