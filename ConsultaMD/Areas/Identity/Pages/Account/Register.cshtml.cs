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
using ConsultaMD.Controllers;
using ConsultaMD.Extensions;
using ConsultaMD.Data;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using ConsultaMD.Hubs;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class RegisterModel : PageModel
    {
        private readonly IHubContext<FeedBackHub> _feedbackHub;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;
        private readonly IFonasa _fonasa;
        private readonly ILookupNormalizer _normalizer;
        private readonly IStringLocalizer<RegisterModel> _localizer;

        public RegisterModel(
            ApplicationDbContext context,
            IHubContext<FeedBackHub> feedbackHub,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<RegisterModel> localizer,
            ILogger<RegisterModel> logger,
            ILookupNormalizer normalizer,
            IEmailSender emailSender,
            IFonasa fonasa,
            IViewRenderService viewRenderService)
        {
            _normalizer = normalizer;
            _feedbackHub = feedbackHub;
            _context = context;
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
            returnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                var rutParsed = RUT.Unformat(Input.RUT);
                var carnetParsing = int.TryParse(Input.CarnetId
                    .Replace(".","", StringComparison.InvariantCulture), out int carnetParsed);
                if (rutParsed.HasValue && carnetParsing)
                {
                    //Validate ID and get nationality
                    await _feedbackHub.Clients.Client(Input.ConnectionId)
                        .SendAsync("FeedBack", "Validando Cédula").ConfigureAwait(true);
                    var nationality = await SPServices.SRCI(rutParsed.Value.rut, carnetParsed)
                        .ConfigureAwait(false);
                    if(nationality != null)
                    {
                        var carnet = new Carnet
                        {
                            Id = carnetParsed,
                            NaturalId = rutParsed.Value.rut
                        };
                        var natural = new Natural
                        {
                            Id = rutParsed.Value.rut,
                            CarnetId = carnet.Id,
                            Carnet = carnet,
                            FullNameFirst = _normalizer.Normalize(Input.Name),
                            Nationality = _normalizer.Normalize(nationality)
                        };
                        //Validate RUT and get Birthday, Names and Sex
                        await _feedbackHub.Clients.Client(Input.ConnectionId)
                            .SendAsync("FeedBack", "Validando datos de paciente").ConfigureAwait(true);
                        var fonasa = await _fonasa.GetById(natural.Id).ConfigureAwait(false);
                        if (fonasa != null)
                        {
                            natural.AddFonasa(fonasa);
                            var pageName = "InsuranceDetails";
                            //IF patient is Fonasa skip insurance and go directly to phone verification
                            if (!string.IsNullOrWhiteSpace(fonasa.ExtGrupoIng))
                            {
                                var patient = new Patient(fonasa)
                                {
                                    NaturalId = natural.Id
                                };
                                natural.Patient = patient;
                                pageName = "VerifyPhone";
                            }
                            //Check if doctor
                            var superData = await SPServices.GetDr(natural.Id).ConfigureAwait(false);
                            if (superData != null)
                            {
                                await _feedbackHub.Clients.Client(Input.ConnectionId)
                                    .SendAsync("FeedBack", "Doctor detectado").ConfigureAwait(true);

                                superData.GetSpecialties =
                                    superData.Specialties
                                    .Select(s => {
                                        var specialty = _context.Specialties.First(sp => sp.Name == _normalizer.Normalize(s));
                                        return new DoctorSpecialty
                                        {
                                            DoctorId = superData.Id,
                                            SpecialtyId = specialty.Id,
                                            Specialty = specialty
                                        };
                                    });
                                var doctor = new Doctor(superData)
                                {
                                    NaturalId = natural.Id
                                };
                                natural.Doctor = doctor;
                                natural.DoctorId = doctor.Id;
                            }
                            //Create user
                            var user = new ApplicationUser
                            {
                                UserName = Input.RUT,
                                Email = Input.Email,
                                Person = natural,
                                PersonId = natural.Id
                            };
                            var result = await _userManager.CreateAsync(user, Input.Password)
                                .ConfigureAwait(false);
                            if (result.Succeeded)
                            {
                                _logger.LogInformation(_localizer["User created a new account with password."]);

                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user)
                                    .ConfigureAwait(false);
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
                                    .RenderToStringAsync("Shared/_ValidationEmail", emailModel)
                                    .ConfigureAwait(false);

                                var response = await _emailSender
                                    .SendEmailAsync(Input.Email, "Verifica tu correo", emailView, logo)
                                    .ConfigureAwait(false);

                                if (response.StatusCode == HttpStatusCode.Accepted)
                                {
                                    user.MailConfirmationTime = DateTime.Now.AddMinutes(5);
                                    await _userManager.UpdateAsync(user).ConfigureAwait(false);
                                    await _signInManager.SignInAsync(user, isPersistent: false)
                                        .ConfigureAwait(false);
                                    return RedirectToPage(pageName, new { returnUrl });
                                }
                                ModelState.AddModelError(string.Empty, 
                                    response.Body.ReadAsStringAsync().Result);
                            }
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                        ModelState.AddModelError(string.Empty, "Error en la validación del paciente");
                    }
                    ModelState.AddModelError(string.Empty, "Error en la validación del carnet");
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
        [Display(Name = "Contraseña (largo min 6 incluyendo letras y números)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string ConnectionId { get; set; }
        [Display(Name="He leído y acepto los Términos y Condiciones")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Por favor acepte los términos y condiciones")]
        public bool Agree { get; set; }
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
