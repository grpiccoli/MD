using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;
using Microsoft.AspNetCore.Hosting;

namespace ConsultaMD.MDs.Controllers
{
    [Area("MDs")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
