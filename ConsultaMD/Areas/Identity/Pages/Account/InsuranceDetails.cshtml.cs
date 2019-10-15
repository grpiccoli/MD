using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Extensions.Validation;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Insurance = ConsultaMD.Data.InsuranceData.Insurance;
using InsuranceValidation = ConsultaMD.Extensions.Validation.Insurance;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class InsuranceDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public InsuranceDetailsModel(UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Previsión")]
            [InsuranceValidation(ErrorMessage = "El RUT no está registrado en la previsión seleccionada")]
            public Insurance Insurance { get; set; }

            public IEnumerable<SelectListItem> InsuranceList { get; set; } = Enum<Insurance>.ToSelect;
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña de su Previsión")]
            [InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
            public string InsurancePassword { get; set; }
            [Required]
            [Extensions.Validation.RUT(ErrorMessage = "RUT no válido")]
            [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
            [Display(Name = "RUT")]
            public string RUT { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            Input = new InputModel { RUT = user.UserName };
            ReturnUrl = returnUrl;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var person = _context.Naturals
                    .Include(n => n.Patient)
                    .SingleOrDefault(p => p.Id == user.PersonId);
                var patient = person.Patient != null ? person.Patient : new Patient();
                patient.Insurance = Input.Insurance;
                patient.InsurancePassword = Input.InsurancePassword;
                person.Patient = patient;
                var result = _context.People.Update(person);
                _logger.LogInformation("Detalles de previsión ingresados.");
                return RedirectToPage("VerifyPhone", new { returnUrl });
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}