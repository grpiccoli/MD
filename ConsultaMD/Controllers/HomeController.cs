using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Globalization;
using ConsultaMD.Services;

namespace ConsultaMD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Patients" });
        }

        [Authorize]
        public IActionResult GetImg(int id)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                "profileImg", id.ToString(CultureInfo.InvariantCulture)+".min.jpg");
            var physical = System.IO.File.Exists(file) ? file : Path.Combine(Directory.GetCurrentDirectory(),DefaultImageMiddleware.DefaultImagePath);
            return PhysicalFile(physical, "image/jpg");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Test()
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
