using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ConsultaMD.Models.Entities;
using Microsoft.Extensions.Localization;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IStringLocalizer<LoginModel> _localizer;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<LoginModel> localizer,
            ILogger<LoginModel> logger)
        {
            _localizer = localizer;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public List<AuthenticationScheme> ExternalLogins { get; } = new List<AuthenticationScheme>();

        public Uri ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(Uri returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= new Uri(Url.Content("~/"), UriKind.Relative);

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);

            var ext = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();

            ExternalLogins.Clear();
            ExternalLogins.AddRange(ext);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            returnUrl ??= new Uri(Url.Content("~/"));

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.RUT, Input.Password, true, lockoutOnFailure: true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(_localizer["Usuario ingresado."]);
                    return LocalRedirect(returnUrl.ToString());
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, v = true });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(_localizer["Usuario bloqueado."]);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "error de usuario / contraseña.");
                    var ex = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
                    ExternalLogins.Clear();
                    ExternalLogins.AddRange(ex);
                    return Page();
                }
            }
            var ext = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
            ExternalLogins.Clear();
            ExternalLogins.AddRange(ext);
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class LoginInputModel
    {
        [Required]
        [Extensions.Validation.RUT(ErrorMessage = "RUT no válido")]
        public string RUT { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
