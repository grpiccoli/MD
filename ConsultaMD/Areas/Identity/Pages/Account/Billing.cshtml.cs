using ConsultaMD.Data;
using ConsultaMD.Extensions.Validation;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class BillingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;
        private readonly IRedirect _redirect;
        public BillingModel(
            IRedirect redirect,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ILogger<RegisterModel> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context
            )
        {
            _redirect = redirect;
            _localizer = localizer;
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        [BindProperty]
        public BillingInputModel Input { get; set; }
        public Uri ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(Uri returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            ReturnUrl = returnUrl;
            Input = new BillingInputModel
            {
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
                    person.PassSII = Input.Password;
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
    public class BillingInputModel
    {
        //[Required]
        //[RUT(ErrorMessage = "RUT no v�lido")]
        //[RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        //[Display(Name = "RUT persona jur�dica")]
        //public string RUTJ { get; set; }
        [Required]
        [RUT(ErrorMessage = "RUT persona natural")]
        [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        [Display(Name = "RUT representante")]
        public string RUT { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña de SII representante")]
        [InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
        public string Password { get; set; }
    }
}
