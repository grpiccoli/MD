using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ConsultaMD.Services;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using ConsultaMD.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Rest.Preview.AccSecurity.Service;
using ConsultaMD.Extensions.Validation;
using Microsoft.EntityFrameworkCore;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    public class VerifyPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        //private readonly TwilioVerifyClient _client;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public VerifyPhoneModel(
            IOptions<TwilioVerifySettings> settings,
            //TwilioVerifyClient client,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            //_client = client;
            _settings = settings?.Value;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        [Required]
        [CellPhone(ErrorMessage = "Ingrese un {0} válido")]
        [Display(Name = "Teléfono Móvil")]
        //[RegularExpression(@"^(?=(?:\D*\d){9})[\(\)\s\-]{,5}$")]
        public string PhoneNumber { get; set; }
        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Patient)
                .Include(u => u.Person)
                    .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception($"No se ha podido cargar el ID de usuario '{_userManager.GetUserId(User)}' Por favor intente limpiar cache e ingresar nuevamente.");
            }
            if(user.Person.Patient == null)
            {
                var pageName = "./InsuranceDetails";
                return RedirectToPage(pageName, new { returnUrl });
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                PhoneNumber = user.PhoneNumber;
            }
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            returnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                var phoneParse = phoneNumberUtil.Parse(PhoneNumber, "CL");
                if (phoneNumberUtil.IsPossibleNumberForType(phoneParse, PhoneNumbers.PhoneNumberType.MOBILE))
                {
                    var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                    if (user == null)
                    {
                        throw new Exception($"No se ha podido cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
                    }
                    if (user.PhoneConfirmationTime > DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, "Código ya enviado, espere 5 minutos antes de enviar otro");
                        return RedirectToPage("ConfirmPhone", new { returnUrl });
                    }
                    var telephone = phoneNumberUtil
                        .FormatOutOfCountryCallingNumber(phoneParse, null)
                        .Replace(" ","", StringComparison.InvariantCulture);
                    if (telephone == "+56968419339") telephone = "+64221699335";
                    user.PhoneNumber = telephone;
                    user.PhoneNumberConfirmed = false;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var verification = await VerificationResource.CreateAsync(
                        to: telephone,
                        channel: "sms",
                        pathServiceSid: _settings.VerificationServiceSID
                    ).ConfigureAwait(false);
                    if (verification.Status == "pending")
                    {
                        user.PhoneConfirmationTime = DateTime.Now.AddMinutes(5);
                        await _userManager.UpdateAsync(user).ConfigureAwait(false);

                        return RedirectToPage("ConfirmPhone", new { ReturnUrl });
                    }
                    ModelState.AddModelError("", $"Hubo un error al enviar el código de verificación: {verification.Status}");
                }
                ModelState.AddModelError(string.Empty, "Error en el formato de Teléfono");
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "Hubo un error al enviar el código de verificación, por favor verifique que el número sea un número de celular válido");
                throw;
            }
            return Page();
        }
    }
}