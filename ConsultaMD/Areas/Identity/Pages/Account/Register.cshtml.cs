using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ConsultaMD.Services;
using IEmailSender = ConsultaMD.Services.IEmailSender;
using SendGrid.Helpers.Mail;
using System.Net;
using ConsultaMD.Extensions.Validation;
using Microsoft.Extensions.Localization;
using ConsultaMD.Extensions;
using ConsultaMD.Data;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using ConsultaMD.Hubs;
using System.Globalization;

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
        private readonly ISuperSaludService _superSalud;
        private readonly IFonasa _fonasa;
        private readonly IRedirect _redirect;
        private readonly ILookupNormalizer _normalizer;
        private readonly IStringLocalizer<RegisterModel> _localizer;
        private readonly IRegCivil _regCivil;

        public RegisterModel(
            ApplicationDbContext context,
            IHubContext<FeedBackHub> feedbackHub,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<RegisterModel> localizer,
            ISuperSaludService superSalud,
            ILogger<RegisterModel> logger,
            IRedirect redirect,
            ILookupNormalizer normalizer,
            IEmailSender emailSender,
            IRegCivil regCivil,
            IFonasa fonasa)
        {
            _superSalud = superSalud;
            _redirect = redirect;
            _regCivil = regCivil;
            _normalizer = normalizer;
            _feedbackHub = feedbackHub;
            _context = context;
            _fonasa = fonasa;
            _localizer = localizer;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                var rutParsed = RUT.Unformat(Input.RUT);
                var carnetParsing = int.TryParse(Input.CarnetId
                    .Replace(".","", StringComparison.InvariantCulture), out int carnetParsed);
                if (rutParsed.HasValue && carnetParsing)
                {
                    var created = _context.Users.SingleOrDefault(p => p.PersonId == rutParsed.Value.rut);
                    if (created != null)
                    {
                        return RedirectToPage("Login", new { ReturnUrl });
                    }
                    //Validate ID *and get nationality* NOT ANYMORE
                    await _feedbackHub.Clients.Client(Input.ConnectionId)
                        .SendAsync("FeedBack", "Validando nuevo usuario").ConfigureAwait(true);
                    var isValid = await _regCivil.IsValidAsync(rutParsed.Value.rut, carnetParsed, Input.IsExt)
                        .ConfigureAwait(false);
                    if (isValid)
                    {
                        await _feedbackHub.Clients.Client(Input.ConnectionId)
                            .SendAsync("FeedBack", "Usuario válido").ConfigureAwait(true);
                        //Check if doctor
                        await _feedbackHub.Clients.Client(Input.ConnectionId)
                            .SendAsync("FeedBack", "Verificando registros médicos").ConfigureAwait(true);
                        var superData = await _superSalud.GetDr(rutParsed.Value.rut).ConfigureAwait(false);
                        var doc = superData != null;
                        if (doc)
                            await _feedbackHub.Clients.Client(Input.ConnectionId)
                                .SendAsync("FeedBack", "Médico detectado").ConfigureAwait(true);
                        //Validate RUT and get Birthday, Names and Sex
                        await _feedbackHub.Clients.Client(Input.ConnectionId)
                            .SendAsync("FeedBack", "Creando ficha médica").ConfigureAwait(true);
                        var fonasa = await _fonasa.GetByIdAsync(rutParsed.Value.rut, doc).ConfigureAwait(false);
                        if (fonasa != null)
                        {
                            await _feedbackHub.Clients.Client(Input.ConnectionId)
                                .SendAsync("FeedBack", "Ficha creada").ConfigureAwait(true);
                            //Add Carnet
                            if (!_context.Carnets.Any(c => c.Id == carnetParsed))
                            {
                                await _context.Carnets.AddAsync(new Carnet
                                {
                                    Id = carnetParsed,
                                    NaturalId = rutParsed.Value.rut
                                }).ConfigureAwait(false);
                            }
                            Natural natural = null;
                            //Add Person
                            if (!_context.Naturals.Any(n => n.Id == rutParsed.Value.rut))
                            {
                                natural = new Natural
                                {
                                    Id = rutParsed.Value.rut,
                                    CarnetId = carnetParsed,
                                    Nationality = _normalizer.NormalizeName(Input.IsExt ? "EXTRANJERA" : "CHILENA")
                                };
                                natural.AddFonasa(fonasa);
                                await _context.Naturals.AddAsync(natural).ConfigureAwait(false);
                                await _context.SaveChangesAsync().ConfigureAwait(false);
                            }
                            else
                            {
                                natural = _context.Naturals.SingleOrDefault(n => n.Id == natural.Id);
                                natural.AddFonasa(fonasa);
                                _context.Naturals.Update(natural);
                                await _context.SaveChangesAsync().ConfigureAwait(false);
                            }
                            //IF patient is Fonasa skip insurance and go directly to phone verification
                            if (!string.IsNullOrWhiteSpace(fonasa.ExtGrupoIng))
                            {
                                //FONASA Patient
                                await _feedbackHub.Clients.Client(Input.ConnectionId)
                                    .SendAsync("FeedBack", "Afiliación a FONASA detectada").ConfigureAwait(true);
                                var patient = new Patient(fonasa)
                                {
                                    NaturalId = natural.Id
                                };
                                if(!_context.Patients.Any(p => p.NaturalId == natural.Id))
                                    await _context.Patients.AddAsync(patient).ConfigureAwait(false);
                                natural.Patient = patient;
                                _context.Naturals.Update(natural);
                                await _context.SaveChangesAsync().ConfigureAwait(false);
                            }
                            if (doc)
                            {
                                _logger.LogInformation(string.Join(" ",superData.Specialties));

                                superData.GetSpecialties =
                                    superData.Specialties
                                    .Select(s => {
                                        var specialty = _context.Specialties
                                        .First(sp => sp.Name == _normalizer
                                        .NormalizeName(s));
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

                                if(!_context.Doctors.Any(d => d.NaturalId == natural.Id))
                                    await _context.Doctors.AddAsync(doctor).ConfigureAwait(false);

                                natural.DoctorId = doctor.Id;
                                _context.Naturals.Update(natural);
                                await _context.SaveChangesAsync().ConfigureAwait(false);

                                //Doctor added to database
                                //var docFonasa = await _fonasa.GetDocDataAsync(natural.Id).ConfigureAwait(false);
                                if (fonasa.Docs.Any())
                                {
                                    await _feedbackHub.Clients.Client(Input.ConnectionId)
                                        .SendAsync("FeedBack", "Convenio FONASA detectado").ConfigureAwait(true);

                                    doctor.FonasaLevel = fonasa.Docs.First().NivelPrestador;

                                    _context.Doctors.Update(doctor);
                                    await _context.SaveChangesAsync().ConfigureAwait(false);

                                    var groups = fonasa.Docs.GroupBy(g => g.Address);
                                    foreach(var g in groups)
                                    {
                                        var filtered = g.Count() > 1 ?
                                            g.Where(g => g.Prestacion.Description != "CONSULTA MEDICA ELECTIVA")
                                            : g;
                                        //CREATE MEDIUM
                                        var medium = new MediumDoctor
                                        {
                                            DoctorId = doctor.Id,
                                        };
                                        await _context.MediumDoctors.AddAsync(medium).ConfigureAwait(false);
                                        await _context.SaveChangesAsync().ConfigureAwait(false);
                                        foreach (var s in filtered)
                                        {
                                            var rut = RUT.Unformat(s.RutPrestador);
                                            if (rut.HasValue)
                                            {
                                                var selector = s.CodEspeciali.ToString(CultureInfo.InvariantCulture);
                                                var person = await _context.People.FindAsync(rut.Value.rut).ConfigureAwait(false);
                                                if(person == null)
                                                {
                                                    //URGENT!!!!! add companies here
                                                    await _context.Companies.AddAsync(new Company
                                                    {
                                                        Id = rut.Value.rut
                                                    }).ConfigureAwait(false);
                                                    await _context.SaveChangesAsync().ConfigureAwait(false);
                                                }
                                                if (!_context.Prestacions.Any(p => p.Id == s.PrestacionId))
                                                {
                                                    await _context.Prestacions.AddAsync(s.Prestacion).ConfigureAwait(false);
                                                    await _context.SaveChangesAsync().ConfigureAwait(false);
                                                }
                                                var agreement = _context.InsuranceAgreements
                                                    .FirstOrDefault(a =>
                                                    a.PersonId == rut.Value.rut
                                                    && a.Insurance == InsuranceData.Insurance.Fonasa);
                                                if (agreement == null)
                                                {
                                                    agreement = new InsuranceAgreement
                                                    {
                                                        Insurance = InsuranceData.Insurance.Fonasa,
                                                        PersonId = rut.Value.rut
                                                    };
                                                    await _context.InsuranceAgreements.AddAsync(agreement).ConfigureAwait(false);
                                                    await _context.SaveChangesAsync().ConfigureAwait(false);
                                                }
                                                if (!_context.Prestacions.Any(p => p.Id == s.PrestacionId))
                                                    await _context.Prestacions
                                                        .AddAsync(s.Prestacion).ConfigureAwait(false);
                                                if (!_context.InsuranceLocations.Any(i =>
                                                   i.InsuranceSelector == selector
                                                && i.PrestacionId == s.PrestacionId
                                                && i.MediumDoctorId == medium.Id
                                                && i.InsuranceAgreementId == agreement.Id))
                                                {
                                                    var ins = new InsuranceLocation
                                                    {
                                                        Address = s.Address,
                                                        InsuranceSelector = selector,
                                                        PrestacionId = s.PrestacionId,
                                                        MediumDoctorId = medium.Id,
                                                        InsuranceAgreementId = agreement.Id,
                                                        CommuneId = int.Parse("1" + s.Commune, CultureInfo.InvariantCulture)
                                                    };
                                                    await _context.InsuranceLocations
                                                        .AddAsync(ins).ConfigureAwait(false);
                                                    medium.InsuranceLocations.Add(ins);
                                                }
                                                await _context.SaveChangesAsync().ConfigureAwait(false);
                                            }
                                        }
                                    }
                                    await _context.SaveChangesAsync().ConfigureAwait(false);
                                    await _feedbackHub.Clients.Client(Input.ConnectionId)
                                        .SendAsync("FeedBack", "Convenios FONASA agregados").ConfigureAwait(true);
                                    //Convenio FONASA Added
                                }
                                else
                                {
                                    if(doc) await _feedbackHub.Clients.Client(Input.ConnectionId)
                                        .SendAsync("FeedBack", "Sin Convenio Fonasa Web, contacte a FONASA").ConfigureAwait(true);
                                }
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
                                await _feedbackHub.Clients.Client(Input.ConnectionId)
                                    .SendAsync("FeedBack", "Usuario creado").ConfigureAwait(true);

                                _logger.LogInformation(_localizer["User created a new account with password."]);

                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user)
                                    .ConfigureAwait(false);
                                var callbackUrl = Url.Page(
                                    "/Account/ConfirmEmail",
                                    pageHandler: null,
                                    values: new { userId = user.Id, code, returnUrl = ReturnUrl },
                                    protocol: Request.Scheme);

                                var response = await _emailSender.SendVerificationEmail(Input.Email, new Uri(callbackUrl)).ConfigureAwait(false);

                                if (response.StatusCode == HttpStatusCode.Accepted)
                                {
                                    await _feedbackHub.Clients.Client(Input.ConnectionId)
                                        .SendAsync("FeedBack", $"Correo de validación enviado a {Input.Email}").ConfigureAwait(true);

                                    user.MailConfirmationTime = DateTime.Now.AddMinutes(5);
                                    var identityResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                                    await _signInManager.SignInAsync(user, isPersistent: false)
                                        .ConfigureAwait(false);
                                    if (identityResult.Succeeded)
                                    {
                                        return await _redirect.Redirect(ReturnUrl, Input.RUT).ConfigureAwait(false);
                                    }
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

        //[Required]
        //[ReadOnly(true)]
        //[Display(Name = "Nombre Completo")]
        //[RegularExpression(@"[A-Za-zÁÉÍÓÚÑÜáéíóúüñ ]+")]
        //public string Name { get; set; }

        [Display(Name = "N° Documento de Carnet")]
        //[Required]
        [RegularExpression(@"[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}")]
        [Carnet(ErrorMessage = "{0} no válido")]
        public string CarnetId { get; set; }

        //[Display(Name = "Nacionalidad")]
        //[ReadOnly(true)]
        //[Required]
        //public string Nationality { get; set; }

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

        [Required]
        [Display(Name="He leído y acepto los Términos y Condiciones")]
        [Compare("IsTrue", ErrorMessage = "Por favor acepte los términos y condiciones")]
        public bool Agree { get; set; }
        public bool IsTrue { get; } = true;
        [Display(Name = "Soy Extranjero")]
        public bool IsExt { get; set; }
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
