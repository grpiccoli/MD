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
using Microsoft.Extensions.Localization;

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
        private readonly IFonasa _fonasa;
        private readonly IStringLocalizer<RegisterModel> _localizer;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<RegisterModel> localizer,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IFonasa fonasa,
            IViewRenderService viewRenderService)
        {
            _fonasa = fonasa;
            _localizer = localizer;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }
        public Uri ReturnUrl { get; set; }

        public void OnGet(Uri returnUrl = null)
        {
            Input = new RegisterInputModel();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            returnUrl = returnUrl ?? new Uri(Url.Content("~/"));
            if (ModelState.IsValid)
            {
                var rutParsed = Extensions.RUT.Unformat(Input.RUT);
                var carnetParsing = int.TryParse(Input.CarnetId.Replace(".","", StringComparison.InvariantCulture), out int carnetParsed);
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
                    var fonasa = await _fonasa.GetById(natural.Id).ConfigureAwait(false);
                    natural.AddFonasa(fonasa);
                    var pageName = "InsuranceDetails";
                    if (string.IsNullOrWhiteSpace(fonasa.ExtGrupoIng)) 
                    {
                        var patient = new Patient
                        {
                            Tramo = Enum.Parse<Tramo>(fonasa.ExtGrupoIng)
                        };
                        natural.Patient = patient;
                        pageName = "VerifyPhone";
                    }
                    var user = new ApplicationUser
                    {
                        UserName = Input.RUT,
                        Email = Input.Email,
                        Person = natural
                    };
                    var result = await _userManager.CreateAsync(user, Input.Password).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localizer["User created a new account with password."]);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
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
                            .RenderToStringAsync("Shared/_ValidationEmail", emailModel).ConfigureAwait(false);

                        var response = await _emailSender
                            .SendEmailAsync(Input.Email, "Verifica tu correo", emailView, logo).ConfigureAwait(false);

                        if(response.StatusCode == HttpStatusCode.Accepted)
                        {
                            user.MailConfirmationTime = DateTime.Now.AddMinutes(5);
                            await _userManager.UpdateAsync(user).ConfigureAwait(false);
                            await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                            return RedirectToPage(pageName, new { returnUrl });
                        }
                        ModelState.AddModelError(string.Empty, response.Body.ReadAsStringAsync().Result);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error en el formato de RUT");
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class RegisterInputModel
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
        [Carnet(ErrorMessage = "{0} no válido")]
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
        [Display(Name = "Contraseña (al menos 6 caracteres y debe contener números letras)")]
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
}
