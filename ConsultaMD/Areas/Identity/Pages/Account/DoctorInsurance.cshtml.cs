using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Extensions.Validation;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class DoctorInsuranceModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;
        private readonly IRedirect _redirect;
        public DoctorInsuranceModel(
            UserManager<ApplicationUser> userManager,
            IRedirect redirect,
            ILogger<RegisterModel> logger,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context
            )
        {
            _redirect = redirect;
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }
        [BindProperty]
        public DoctorInsuranceInputModel Input { get; set; }
        //Exclude FONASA, added during registry
        public string InsuranceList { get; } = JsonConvert.SerializeObject(EnumUtils.Enum2Ms<Insurance>("Name").Where(i => i.value > 1));
        public Uri ReturnUrl { get; set; }
        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            ReturnUrl = returnUrl;
            Input = new DoctorInsuranceInputModel {
                RUT = user.UserName
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var person = _context.Naturals
                    .Include(n => n.Patient)
                    .Include(n => n.Doctor)
                    .SingleOrDefault(p => p.Id == user.PersonId);
                if (person.Doctor != null)
                {
                    //FALTAN CONVENIOS PREVISIONES
                    var result = _context.People.Update(person);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.LogInformation(_localizer["Detalles de previsión ingresados."]);
                    return await _redirect.Redirect(ReturnUrl, Input.RUT).ConfigureAwait(false);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class DoctorInsuranceInputModel
    {
        [Required]
        [RUT(ErrorMessage = "RUT no válido")]
        [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        [Display(Name = "RUT")]
        public string RUT { get; set; }
        [Required]
        [Display(Name = "Selecciona tu Convenio")]
        [Insurance(ErrorMessage = "El RUT no está registrado en la previsión seleccionada")]
        public Insurance Company { get; set; }
        //public IEnumerable<SelectListItem> InsuranceList { get; set; } = EnumUtils.Enum2Select<Insurance>("Name").Where(e => e.Value != "1");
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña de su Previsión")]
        [InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
        public string Password { get; set; }
    }
}
