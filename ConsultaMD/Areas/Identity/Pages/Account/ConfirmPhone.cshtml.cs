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
            _settings = settings.Value;
            _userManager = userManager;
        }

        public string PhoneNumber { get; set; }
        public string ReturnUrl { get; set; }
        public DateTime Wait { get; set; }

        [BindProperty, Required, Display(Name = "C�digo")]
        public string VerificationCode { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            await LoadPhoneNumber();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await LoadPhoneNumber();
            returnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                  to: PhoneNumber,
                  code: VerificationCode.Replace(" ",""),
                  pathServiceSid: _settings.VerificationServiceSID
              );
                if (verification.Status == "approved")
                {
                    var identityUser = await _userManager.GetUserAsync(User);
                    identityUser.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(identityUser);

                    if (updateResult.Succeeded)
                    {
                        //return LocalRedirect(returnUrl);
                        return RedirectToPage("ConfirmPhoneSuccess", new { returnUrl });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hubo un error al confirmar su c�digo de verificaci�n, por favor int�ntelo nuevamente");
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"Hubo un error al confirmar su c�digo de verificaci�n: {verification.Status}");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "Hubo un error confirmando el c�digo, por favor verifique el c�digo sea correcto e int�ntelo nuevamente");
            }

            return Page();
        }

        private async Task LoadPhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception($"Error al cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumber = user.PhoneNumber;
            Wait = user.PhoneConfirmationTime;
        }
    }
}