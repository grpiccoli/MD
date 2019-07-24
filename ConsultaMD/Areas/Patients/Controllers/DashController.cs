using ConsultaMD.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients")]
    //[Authorize]
    public class DashController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetLocalities()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
