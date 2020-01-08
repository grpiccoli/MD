using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMedium _medium;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IRedirect _redirect;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;

        public DoctorLocationsModel(UserManager<ApplicationUser> userManager,
            IMedium medium,
            ILogger<RegisterModel> logger,
            IRedirect redirect,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context)
        {
            _medium = medium;
            _redirect = redirect;
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }
        [BindProperty]
        public DoctorLocationsInputModel Input { get; set; }
        //Exclude FONASA, added during registry
        public string SelectorList { get; set; }
        public int? Default { get; set; }
        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null, int? mId = 0)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            SelectorList = JsonConvert.SerializeObject(
                _context.InsuranceLocations
                .Include(i => i.InsuranceAgreement)
                .Include(i => i.MediumDoctor)
                    .ThenInclude(m => m.Doctor)
                .Include(i => i.Commune)
                .Include(i => i.Prestacion)
                .Where(i => i.MediumDoctor.Doctor.NaturalId == user.PersonId)
                .Select(i =>
            new MsSelect{ 
                value = i.Id,
                text = i.GetName()
            }));
            Default = mId;
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
                return await _redirect.Redirect(ReturnUrl, User.Identity.Name).ConfigureAwait(false);
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
    public class DoctorLocationsInputModel
    {
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
