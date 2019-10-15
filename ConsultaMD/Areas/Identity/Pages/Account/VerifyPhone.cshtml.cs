using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ConsultaMD.Services;
using System.Text.RegularExpressions;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using ConsultaMD.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Rest.Preview.AccSecurity.Service;
using ConsultaMD.Extensions.Validation;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    public class VerifyPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        //private readonly TwilioVerifyClient _client;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public VerifyPhoneModel(
            IOptions<TwilioVerifySettings> settings,
            //TwilioVerifyClient client,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            //_client = client;
            _settings = settings.Value;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        [Required]
        [CellPhone(ErrorMessage = "Ingrese un {0} válido")]
        [Display(Name = "Teléfono Móvil")]
        //[RegularExpression(@"^(?=(?:\D*\d){9})[\(\)\s\-]{,5}$")]
        public string PhoneNumber { get; set; }
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception($"No se ha podido cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                PhoneNumber = user.PhoneNumber.Replace("+56", "").Insert(1, " ").Insert(6, " ");
            }
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var phoneParse = int.TryParse(Regex.Replace(PhoneNumber, @"\D", ""), out var phone);
                if (phoneParse)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        throw new Exception($"No se ha podido cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
                    }
                    if (user.PhoneConfirmationTime > DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, "Código ya enviado, espere 5 minutos antes de enviar otro");
                        return RedirectToPage("ConfirmPhone", new { returnUrl });
                    }
                    var telephone = $"+56{phone}";
                    user.PhoneNumber = telephone;
                    user.PhoneNumberConfirmed = false;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    var verification = await VerificationResource.CreateAsync(
                        to: telephone,
                        channel: "sms",
                        pathServiceSid: _settings.VerificationServiceSID
                    );
                    if (verification.Status == "pending")
                    {
                        user.PhoneConfirmationTime = DateTime.Now.AddMinutes(5);
                        await _userManager.UpdateAsync(user);

                        return RedirectToPage("ConfirmPhone", new { returnUrl });
                    }
                    ModelState.AddModelError("", $"Hubo un error al enviar el código de verificación: {verification.Status}");
                }
                ModelState.AddModelError(string.Empty, "Error en el formato de Teléfono");
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "Hubo un error al enviar el código de verificación, por favor verifique que el número sea un número de celular válido");
            }
            return Page();
        }
    }
}