using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IEmailSender = ConsultaMD.Services.IEmailSender;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;

        public ForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IViewRenderService viewRenderService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name="RUT")]
            public string RUT { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.RUT);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                var logo = "wwwroot/android-chrome-256x256.png";

                var emailModel = new ValidationEmailVM
                {
                    Url = new HtmlString(callbackUrl),
                    Color = "19989d",
                    LogoId = "logo",
                    Header = new HtmlString("Nos solicitaste recuperar tu contraseña"),
                    Body = new HtmlString("Has click en el siguiente enlace para restaurar tu contraseña.<br><p style='font-size:12px'>*Si no solicitaste restaurar tu contraseña reporta este correo a soporte@consultamd.cl</p>"),
                    ButtonTxt = new HtmlString("Restaurar contraseña")
                };

                var emailView = await _viewRenderService
                    .RenderToStringAsync("Shared/_ValidationEmail", emailModel);

                var maskedEmail = Masking.MaskEmail(user.Email);
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Recuperar Contraseña",emailView, logo);
                return RedirectToPage("./ForgotPasswordConfirmation", new { maskedEmail });
            }

            return Page();
        }
    }
}
