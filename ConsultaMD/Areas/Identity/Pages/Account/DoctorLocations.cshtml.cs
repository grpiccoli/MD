using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IStringLocalizer<InsuranceDetailsModel> _localizer;

        public DoctorLocationsModel(UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IStringLocalizer<InsuranceDetailsModel> localizer,
            ApplicationDbContext context)
        {
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }
        [BindProperty]
        public DoctorLocationsInputModel Input { get; set; }
        //Exclude FONASA, added during registry
        public string SelectorList { get; set; }
        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
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
            ReturnUrl = returnUrl;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl ?? new Uri(Url.Content("~/"), UriKind.Relative);
            if (ModelState.IsValid)
            {
                var insuranceLocation = await _context.InsuranceLocations
                    .Include(i => i.MediumDoctor)
                    .FirstOrDefaultAsync(i => i.Id == Input.Selector).ConfigureAwait(false);
                if(insuranceLocation != null)
                {
                    var medicalOffice = await _context.MedicalOffices
                        .FirstOrDefaultAsync(o => 
                        o.PlaceId == Input.PlaceId
                        && o.Block == Input.Block
                        && o.Floor == Input.Floor
                        && o.Appartment == Input.Appartment
                        && o.Office == Input.Office).ConfigureAwait(false);
                    if (medicalOffice == null)
                    {
                        medicalOffice = new MedicalOffice
                        {
                            PlaceId = Input.PlaceId,
                            Block = Input.Block,
                            Floor = Input.Floor,
                            Appartment = Input.Appartment,
                            Office = Input.Office,
                        };
                        _context.MedicalOffices.Add(medicalOffice);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    insuranceLocation.MediumDoctor.MedicalAttentionMediumId = medicalOffice.Id;
                    _context.InsuranceLocations.Update(insuranceLocation);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.LogInformation(_localizer["Detalles de previsión ingresados."]);
                    return RedirectToPage("VerifyPhone", new { ReturnUrl });
                }
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
        public string Block { get; set; }
        public string Floor { get; set; }
        public string Appartment { get; set; }
        public string Office { get; set; }
    }
}
