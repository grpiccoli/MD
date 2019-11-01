using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConsultaMD.Models;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;

namespace ConsultaMD.Controllers
{
    public class HomeController : Controller
    {
        private readonly INodeServices _nodeServices;
        public HomeController(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Patients" });
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
