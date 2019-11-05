using System;
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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class InsuranceDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;

        public InsuranceDetailsModel(UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context)
        {
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InsuranceDetailsInputModel Input { get; set; }
        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            Input = new InsuranceDetailsInputModel { 
                RUT = user.UserName
            };
            ReturnUrl = returnUrl;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            returnUrl = returnUrl ?? new Uri(Url.Content("~/"));
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var person = _context.Naturals
                    .Include(n => n.Patient)
                    .SingleOrDefault(p => p.Id == user.PersonId);
                var patient = person.Patient ?? new Patient();
                patient.Insurance = Input.Insurance;
                patient.InsurancePassword = Input.InsurancePassword;
                person.Patient = patient;
                if(patient.NaturalId == 0)
                {
                    patient.NaturalId = person.Id;
                    var pResult = await _context.Patients.AddAsync(patient).ConfigureAwait(false);
                }
                var result = _context.People.Update(person);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                _logger.LogInformation(_localizer["Detalles de previsión ingresados."]);
                return RedirectToPage("VerifyPhone", new { returnUrl });
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class InsuranceDetailsInputModel
    {
        [Required]
        [Display(Name = "Previsión")]
        [Insurance(ErrorMessage = "El RUT no está registrado en la previsión seleccionada")]
        public Insurance Insurance { get; set; }

        public IEnumerable<SelectListItem> InsuranceList { get; set; } = EnumUtils.Enum2Select<Insurance>("Name").Where(e => e.Value != "1");
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña de su Previsión")]
        [InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
        public string InsurancePassword { get; set; }
        [Required]
        [RUT(ErrorMessage = "RUT no válido")]
        [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        [Display(Name = "RUT")]
        public string RUT { get; set; }
    }
}