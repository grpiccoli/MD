using System.Threading.Tasks;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaMD.Controllers
{
    //Médical Insurance Password
    [Authorize]
    public class MIPController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMIP _fonasa;
        public MIPController(UserManager<ApplicationUser> userManager,
            IMIP fonasa)
        {
            _fonasa = fonasa;
            _userManager = userManager;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(int insurance, string pwd)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var run = RUT.Unformat(user.UserName);
            var result = false;
            if (run.HasValue)
            {
                var rut = run.Value.rut;
                result = await _fonasa.Validate(insurance, rut, pwd).ConfigureAwait(false);
            }
            if (result) return Ok();
            return NotFound();
        }
    }
}