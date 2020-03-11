using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class DoctorLocationsModel : PageModel
    {
        private readonly IMedium _medium;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IRedirect _redirect;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;

        public DoctorLocationsModel(
            IMedium medium,
            ILogger<RegisterModel> logger,
            IRedirect redirect,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context)
        {
            _medium = medium;
            _redirect = redirect;
            _localizer = localizer;
            _logger = logger;
            _context = context;
        }
        [BindProperty]
        public DoctorLocationsInputModel Input { get; set; }
        //Exclude FONASA, added during registry
        public string SelectorList { get; set; }
        public int? Default { get; set; }
        public Uri ReturnUrl { get; set; }
        public bool LocationsAny { get; set; }
        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null, int? mId = null)
        {
            var user = await _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Doctor)
                    .ThenInclude(d => d.MediumDoctors)
                        .ThenInclude(m => m.MedicalAttentionMedium)
            .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name)
            .ConfigureAwait(false);

            LocationsAny = user.Person.Doctor.MediumDoctors
                .Any(m => m.MedicalAttentionMedium != null);

            var ils = _context.InsuranceLocations
                .Include(i => i.InsuranceAgreement)
                .Include(i => i.MediumDoctor)
                    .ThenInclude(m => m.Doctor)
                .Include(i => i.MediumDoctor)
                    .ThenInclude(m => m.MedicalAttentionMedium)
                .Include(i => i.Commune)
                .Include(i => i.Prestacion)
                .Where(i => i.MediumDoctor.Doctor.NaturalId == user.PersonId && i.MediumDoctor.MedicalAttentionMedium == null)
                .Select(i => new MsSelect
                {
                    Value = i.Id,
                    Text = i.GetName()
                });

            SelectorList = JsonConvert.SerializeObject(ils);
            Default = mId.HasValue ? mId : ils.First().Value;
            ReturnUrl = returnUrl;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                await _medium.Add(Input).ConfigureAwait(false);
                _logger.LogInformation(_localizer["Detalles de previsión ingresados."]);
            }
            if (!Input.More)
                return await _redirect.Redirect(ReturnUrl, User.Identity.Name).ConfigureAwait(false);
            var user = await _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Doctor)
                    .ThenInclude(d => d.MediumDoctors)
                        .ThenInclude(m => m.MedicalAttentionMedium)
            .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name)
            .ConfigureAwait(false);

            LocationsAny = user.Person.Doctor.MediumDoctors
                .Any(m => m.MedicalAttentionMedium != null);

            var ils = _context.InsuranceLocations
                .Include(i => i.InsuranceAgreement)
                .Include(i => i.MediumDoctor)
                    .ThenInclude(m => m.Doctor)
                .Include(i => i.MediumDoctor)
                    .ThenInclude(m => m.MedicalAttentionMedium)
                .Include(i => i.Commune)
                .Include(i => i.Prestacion)
                .Where(i => i.MediumDoctor.Doctor.NaturalId == user.PersonId && i.MediumDoctor.MedicalAttentionMedium == null)
                .Select(i => new MsSelect
                {
                    Value = i.Id,
                    Text = i.GetName()
                });
            SelectorList = JsonConvert.SerializeObject(ils);
            Default = ils.First().Value;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
    public class DoctorLocationsInputModel
    {
        public bool More { get; set; }
        //Fonasa Código de prestación
        public int Selector { get; set; }
        public string PlaceId { get; set; }
        [Display(Name = "Valor Particular")]
        public int Price { get; set; }
        [Display(Name = "¿Tiene sobre cupos?")]
        public bool HasOverTime { get; set; } = true;
        [Display(Name = "Bloque")]
        public string Block { get; set; }
        [Display(Name = "Piso")]
        public string Floor { get; set; }
        [Display(Name = "Departamento")]
        public string Appartment { get; set; }
        [Display(Name = "Oficina")]
        public string Office { get; set; }
    }
}
