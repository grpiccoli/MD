using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;
using Microsoft.AspNetCore.Hosting;

namespace ConsultaMD.Patients.Controllers
{
    [Area("Patients")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index2()
        {
            return View();
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
