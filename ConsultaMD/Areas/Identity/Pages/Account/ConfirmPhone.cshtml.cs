using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ConsultaMD.Services;
using Microsoft.Extensions.Options;
using Twilio.Rest.Preview.AccSecurity.Service; 
using ConsultaMD.Models.Entities;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    public class ConfirmPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmPhoneModel(IOptions<TwilioVerifySettings> settings, UserManager<ApplicationUser> userManager)
        {
            _settings = settings?.Value;
            _userManager = userManager;
        }

        public string PhoneNumber { get; set; }
        public Uri ReturnUrl { get; set; }
        public DateTime Wait { get; set; }

        [BindProperty, Required, Display(Name = "Código")]
        public string VerificationCode { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl;
            await LoadPhoneNumber().ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            await LoadPhoneNumber().ConfigureAwait(false);
            returnUrl = returnUrl ?? new Uri(Url.Content("~/"));
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                  to: PhoneNumber,
                  code: VerificationCode.Replace(" ","", StringComparison.InvariantCulture),
                  pathServiceSid: _settings.VerificationServiceSID
              ).ConfigureAwait(false);
                if (verification.Status == "approved")
                {
                    var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                    identityUser.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(identityUser).ConfigureAwait(false);

                    if (updateResult.Succeeded)
                    {
                        //return LocalRedirect(returnUrl);
                        return RedirectToPage("ConfirmPhoneSuccess", new { returnUrl });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hubo un error al confirmar su código de verificación, por favor inténtelo nuevamente");
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"Hubo un error al confirmar su código de verificación: {verification.Status}");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "Hubo un error confirmando el código, por favor verifique el código sea correcto e inténtelo nuevamente");
                throw;
            }

            return Page();
        }

        private async Task LoadPhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception($"Error al cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumber = user.PhoneNumber;
            Wait = user.PhoneConfirmationTime;
        }
    }
}