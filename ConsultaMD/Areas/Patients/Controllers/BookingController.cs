﻿using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => md.Doctor)
                                        .ThenInclude(d => d.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => (MedicalOffice)md.MedicalAttentionMedium)
                                        .ThenInclude(mo => mo.Place)
                        .SingleOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);

                    var paymentvm = new ReservationDetails(reservation);
                    return View(paymentvm);
                }
            }
            return RedirectToAction("Map", "Search", new { area = "Patients" });
        }
        public async Task<IActionResult> Appointments()
        {
            var rut = RUT.Unformat(User.Identity.Name);
            var reservations = await _context.Reservations
                        .Include(r => r.Patient)
                            .ThenInclude(p => p.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => md.Doctor)
                                        .ThenInclude(d => d.Natural)
                        .Include(r => r.TimeSlot)
                            .ThenInclude(ts => ts.Agenda)
                                .ThenInclude(a => a.MediumDoctor)
                                    .ThenInclude(md => (MedicalOffice)md.MedicalAttentionMedium)
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