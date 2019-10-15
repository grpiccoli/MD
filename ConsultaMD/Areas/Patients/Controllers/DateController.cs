using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;
using Microsoft.AspNetCore.Hosting;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class DateController : Controller
    {
        public IActionResult WaitingRoom()
        {
            return View();
        }
    }
}

