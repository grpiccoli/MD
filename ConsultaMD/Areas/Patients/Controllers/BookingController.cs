using ConsultaMD.Data;
using ConsultaMD.Models.VM.PatientsVM;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ConfirmPayment([Bind(
            "RutCliente, NombreCliente, Dia, Hora, Direccion, RutDoctor, DoctorId, " +
            "Lugar, NombreDoctor, Especialidad, EspecialidadId, Address, PlaceId")]
        PaymentVM model)
        {
            ViewData["run"] = User.Identity.Name.Replace(".", "").Split("-")[0];

            if (ModelState.IsValid)
            {
                return View(model);
            }
            return RedirectToAction("Map", "Search", new { area = "Patients" });
        }
        public JsonResult BookingMorning(string date, string placeId, int drId)
        {
            var morning = new[]
            {
                new {hora="9:00"},
                new {hora="9:10"},
                new {hora="9:20"},
                new {hora="9:30"},
                new {hora="9:40"},
                new {hora="9:50"},
                new {hora="10:00"},
                new {hora="10:10"},
                new {hora="10:20"},
                new {hora="10:30"},
                new {hora="10:40"},
                new {hora="10:50"},
                new {hora="11:00"},
                new {hora="14:10"},
                new {hora="14:20"},
                new {hora="14:30"},
                new {hora="14:40"},
                new {hora="14:50"}
            };
            return Json(morning);
        }
        public IActionResult BookAppointment()
        {
            ViewData["Message"] = "Hello";
            ViewData["NumTimes"] = 150;
            return View();
        }

        public IActionResult CancelBookingDoctor()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ReservationSummary([Bind(
            "RutCliente, NombreCliente, Dia, Hora, Direccion, RutDoctor, DoctorId, " +
            "Lugar, NombreDoctor, Especialidad, EspecialidadId, Address, PlaceId, PaymentType")] PaymentVM model)
        {
            ViewData["run"] = User.Identity.Name.Replace(".", "").Split("-")[0];
            if (ModelState.IsValid)
            {
                //return RedirectToAction("Sbono", "Consalud", new
                //{
                //    rutp = model.RutCliente,
                //    rutdr = model.RutDoctor,
                //    placeId = model.PlaceId
                //}
                //);
                return View(model);
            }
            return View("Error");
        }
        public IActionResult TransactionHistory()
        {
            return View();
        }

    }
}