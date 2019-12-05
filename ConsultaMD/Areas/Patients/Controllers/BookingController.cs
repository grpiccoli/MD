using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.VM;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public BookingController(ApplicationDbContext context,
            IFonasa fonasa)
        {
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
        public async Task<IActionResult> Payment(string Rut, int Id, int Sp)
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
                                    var paymentData = new PaymentData
                                    {
                                        Commune = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Commune.GetCUT(),
                                        Region = reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.MedicalAttentionMedium.Place.Commune.Province.Region.GetCUT(),
                                        DocRut = RUT.Fonasa(reservation.TimeSlot.Agenda.AgendaEvent.MediumDoctor.Doctor.NaturalId),
                                        Email = user.Email,
                                        PayRut = RUT.Fonasa(Rut),
                                        Phone = user.PhoneNumber.Replace("+56", "", System.StringComparison.InvariantCulture),
                                        Rut = RUT.Fonasa(user.PersonId),
                                        Specialty = Sp.ToString("d", null)
                                        //reservation.TimeSlot.Agenda.MediumDoctor.Doctor.Specialty.Value.ToString("d")
                                    };
                                    var fonasaWebPay = await _fonasa.Pay(paymentData).ConfigureAwait(false);
                                    return Ok(fonasaWebPay.TokenWs);
                            }
                        }
                        else
                        {

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