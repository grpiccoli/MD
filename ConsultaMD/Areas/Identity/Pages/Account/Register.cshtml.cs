using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using ConsultaMD.Services;
using IEmailSender = ConsultaMD.Services.IEmailSender;
using SendGrid.Helpers.Mail;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.Html;
using System.Net;
using ConsultaMD.Extensions.Validation;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IViewRenderService viewRenderService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [RUT(ErrorMessage = "RUT no válido")]
            [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
            //[Natural]
            [Display(Name = "RUT")]
            public string RUT { get; set; }

            [Required]
            [ReadOnly(true)]
            [Display(Name = "Nombre Completo")]
            [RegularExpression(@"[A-Za-zÁÉÍÓÚÑÜáéíóúüñ ]+")]
            public string Name { get; set; }

            [Display(Name = "N° Documento de Carnet")]
            //[Required]
            [RegularExpression(@"[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}")]
            [CarnetId(ErrorMessage = "{0} no válido")]
            public string CarnetId { get; set; }

            [Display(Name = "Nacionalidad")]
            [ReadOnly(true)]
            [Required]
            public string Nationality { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres de largo.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }

            //[Required]
            //[Display(Name = "Previsión")]
            //public Insurance Insurance { get; set; }

            //public IEnumerable<SelectListItem> InsuranceList { get; set; } = Enum<Insurance>.ToSelect;

            //[DataType(DataType.Password)]
            //[Display(Name = "Contraseña de su Previsión")]
            //[InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
            //public string InsurancePassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            Input = new InputModel();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var rutParsed = Extensions.RUT.Unformat(Input.RUT);
                var carnetParsing = int.TryParse(Input.CarnetId.Replace(".",""), out int carnetParsed);
                if (rutParsed.HasValue && carnetParsing)
                {
                    var carnet = new Carnet
                    {
                        Id = carnetParsed
                    };
                    var natural = new Natural
                    {
                        Id = rutParsed.Value.rut,
                        Carnet = carnet,
                        FullNameFirst = Input.Name
                    };
                    var user = new ApplicationUser
                    {
                        UserName = Input.RUT,
                        Email = Input.Email,
                        Person = natural
                    };
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code },
                            protocol: Request.Scheme);

                        var logo = "wwwroot/android-chrome-256x256.png";

                        var emailModel = new ValidationEmailVM
                        {
                            Header = new HtmlString("¡Estás a un paso de tu atención médica!<br>Confirma tu correo."),
                            Url = new HtmlString(callbackUrl),
                            Color = "19989d",
                            LogoId = "logo",
                            Body = new HtmlString("Al hacer click en el siguiente enlace, estás confirmando tu correo."),
                            ButtonTxt = new HtmlString("Confirmar Correo")
                        };

                        var emailView = await _viewRenderService
                            .RenderToStringAsync("Shared/_ValidationEmail", emailModel);

                        var response = await _emailSender
                            .SendEmailAsync(Input.Email, "Verifica tu correo", emailView, logo);

                        if(response.StatusCode == HttpStatusCode.Accepted)
                        {
                            user.MailConfirmationTime = DateTime.Now.AddMinutes(5);
                            await _userManager.UpdateAsync(user);
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            return RedirectToPage("InsuranceDetails", new { returnUrl });
                        }
                        ModelState.AddModelError(string.Empty, response.Body.ReadAsStringAsync().Result);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                ModelState.AddModelError(string.Empty, "Error en el formato de RUT");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
