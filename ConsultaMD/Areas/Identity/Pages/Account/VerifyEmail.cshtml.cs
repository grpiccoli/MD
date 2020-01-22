using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public VerifyEmailModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        [BindProperty]
        [Required]
        [EmailAddress(ErrorMessage = "Ingrese un {0} válido")]
        [Display(Name = "Correo electrónico")]
        //[RegularExpression(@"^(?=(?:\D*\d){9})[\(\)\s\-]{,5}$")]
        public string Email { get; set; }
        public Uri ReturnUrl { get; set; }
        public TimeSpan Wait { get; set; }
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
            if (user.Person.Patient == null)
            {
                var pageName = "./InsuranceDetails";
                return RedirectToPage(pageName, new { returnUrl });
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                if (user.EmailConfirmed) return LocalRedirect(returnUrl?.ToString());
                Email = user.Email;
            }
            ReturnUrl = returnUrl;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null)
                {
                    throw new Exception($"No se ha podido cargar el ID de usuario '{_userManager.GetUserId(User)}'.");
                }
                if (user.MailConfirmationTime > DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "Correo ya enviado, espere 5 minutos antes de enviar otro");
                    Wait = DateTime.Now - user.MailConfirmationTime;
                    return Page();
                }
                user.EmailConfirmed = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user)
                    .ConfigureAwait(false);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, code },
                    protocol: Request.Scheme);

                var response = await _emailSender.SendVerificationEmail(Email, new Uri(callbackUrl)).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    user.MailConfirmationTime = DateTime.Now.AddMinutes(5);
                    await _userManager.UpdateAsync(user).ConfigureAwait(false);
                    await _signInManager.SignInAsync(user, isPersistent: false)
                        .ConfigureAwait(false);
                    return Page();
                }
                ModelState.AddModelError(string.Empty,
                    response.Body.ReadAsStringAsync().Result);
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
