using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Extensions.Validation;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class InsuranceDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMIP _mIPService;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;

        public InsuranceDetailsModel(
            IMIP mIPService,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context)
        {
            _mIPService = mIPService;
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InsuranceDetailsInputModel Input { get; set; }
        public Uri ReturnUrl { get; set; }
        private Redirect Redir { get; set; } = new Redirect();
        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            Input = new InsuranceDetailsInputModel { 
                RUT = user.UserName
            };
            ReturnUrl = returnUrl;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var person = _context.Naturals
                    .Include(n => n.Patient)
                    .Include(n => n.Doctor)
                    .SingleOrDefault(p => p.Id == user.PersonId);
                if(person != null)
                {
                    var valid = await _mIPService.Validate((int)Input.Insurance, person.Id, Input.InsurancePassword)
                        .ConfigureAwait(false);
                    if (valid)
                    {
                        Redir.Doctor = person.Doctor != null;
                        if (person.Patient == null)
                        {
                            person.Patient = new Patient
                            {
                                NaturalId = person.Id,
                                Natural = person
                            };
                        }
                        person.Patient.Insurance = Input.Insurance;
                        person.Patient.InsurancePassword = Input.InsurancePassword;
                        if (_context.Patients.Any(p => p.NaturalId == person.Id))
                        {
                            _context.Patients.Update(person.Patient);
                        }
                        else
                        {
                            await _context.Patients.AddAsync(person.Patient).ConfigureAwait(false);
                        }
                        var result = _context.People.Update(person);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                        Redir.Prevision = true;
                        _logger.LogInformation(_localizer["Detalles de previsión ingresados."]);
                        return RedirectToPage(Redir.GetPage(), new { ReturnUrl });
                    }
                    ModelState.AddModelError(string.Empty, "Error combinación Usuario/Previsión/Contraseña");
                }
                ModelState.AddModelError(string.Empty, "Error desconocido por favor contactese con soporte");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class InsuranceDetailsInputModel
    {
        [Required]
        [Display(Name = "Previsión")]
        [Insurance(ErrorMessage = "El RUT no está registrado en la previsión seleccionada")]
        public Insurance Insurance { get; set; }

        public string InsuranceList { get; } = JsonConvert.SerializeObject(EnumUtils.Enum2Ms<Insurance>("Name").Where(e => e.value != 1));
        //public IEnumerable<SelectListItem> InsuranceList { get; set; } = EnumUtils.Enum2Select<Insurance>("Name").Where(e => e.Value != "1");
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña de su Previsión")]
        [InsurancePassword(ErrorMessage = "Error en la combinación Previsión/{0}")]
        public string InsurancePassword { get; set; }
        [Required]
        [RUT(ErrorMessage = "RUT no válido")]
        [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        [Display(Name = "RUT")]
        public string RUT { get; set; }
    }
}