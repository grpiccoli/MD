using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Map", "Search", new { area = "Patients" });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
