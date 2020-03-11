using ConsultaMD.Data;
using ConsultaMD.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class RedirectService : PageModel, IRedirect
    {
        private readonly ApplicationDbContext _context;
        public RedirectService(
            ApplicationDbContext context
            )
        {
            _context = context;
        }
        public async Task<IActionResult> Redirect(Uri returnUrl, string rut)
        {
            var url = returnUrl?.ToString();
            if (!string.IsNullOrWhiteSpace(url) && RUT.IsValid(rut))
            {
                var page = await Init(RUT.Format(rut)).ConfigureAwait(false);
                if (page == "Done")
                {
                    var urlArray = url.Split("/").Where(t => !string.IsNullOrWhiteSpace(t));
                    if (urlArray.Count() == 3)
                    {
                        return RedirectToAction(urlArray.ElementAt(2), urlArray.ElementAt(1), new { area = urlArray.ElementAt(0) });
                    }
                    return RedirectToAction("Map", "Search", new { area = "Patients" });
                }
                return RedirectToPage(page, new { returnUrl });
            }
            return NotFound();
        }
        public async Task<string> Init(string userName)
        {
            var user = await _context.Users
            .Include(u => u.Person)
                .ThenInclude(p => p.Customer)
            .Include(u => u.Person)
                .ThenInclude(p => p.Patient)
            .Include(u => u.Person)
                .ThenInclude(p => p.Doctor)
                    .ThenInclude(d => d.MediumDoctors)
                        .ThenInclude(m => m.MedicalAttentionMedium)
            .SingleOrDefaultAsync(u => u.UserName == userName)
            .ConfigureAwait(false);

            Prevision = user.Person?.Patient != null;
            Doctor = user.Person?.Doctor != null;
            if (Doctor)
            {
                ConvenioAny = user.Person.Doctor.MediumDoctors.Any();
                LocationAny = user.Person.Doctor.MediumDoctors
                    .Any(m => m.MedicalAttentionMedium != null);
                SiiPassAdded = !string.IsNullOrWhiteSpace(user.Person.PassSII);
                //CardAdded = user.Person.Customer != null;
                CardAdded = true;
            }
            PhoneConfirmed = user.PhoneNumberConfirmed;
            EmailConfirmed = user.EmailConfirmed;
            return GetPage();
        }
        public bool Prevision { get; set; }
        public bool Doctor { get; set; }
        public bool ConvenioAny { get; set; }
        public bool LocationAny { get; set; }
        public bool SiiPassAdded { get; set; }
        public bool CardAdded { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> PageName { get; } = new List<string> {
            "InsuranceDetails",
            "DoctorInsurance",
            "DoctorLocations",
            "Billing",
            "CardDetails",
            "VerifyPhone",
            "VerifyEmail",
            "Done"
        };
        public string GetPage()
        {
            var index =
                Prevision ?
                    //Prevision TRUE
                    (Doctor ?
                        //Prevision + Doctor TRUE
                        (ConvenioAny ?
                            //Prevision + Doctor + ConvenioAny TRUE
                            (LocationAny ?
                                //Prevision + Doctor + ConvenioAny + LocationAny TRUE
                                (SiiPassAdded ?
                                    (CardAdded ?
                                        (PhoneConfirmed ?
                                            //Prevision + Doctor + Convenio + Location + Phone TRUE
                                            (EmailConfirmed ?
                                                    //Prevision + Doctor + Convenio + Location + Phone + Email TRUE
                                                    7
                                                //Prevision + Doctor + Convenio + Location + Phone TRUE email false
                                                : 6
                                            )
                                            //Prevision + Doctor + Convenio + Location TRUE Phone + Email FALSE
                                            : 5
                                        )
                                        //Prevision + Doctor + Convenio TRUE Location + Phone + Email FALSE
                                        : 4
                                    )
                                    : 3
                                )
                                : 2
                            )
                            //Prevision + Doctor TRUE Convenio + Location + Phone + Email FALSE
                            : 1
                        )
                        //Previsión TRUE Doctor FALSE
                        : PhoneConfirmed ?
                            //Previsión + Phone TRUE Doctor False
                            (EmailConfirmed ?
                                    //Previsión + Phone + Email TRUE Doctor FALSE
                                    7
                                //Previsión + Phone TRUE Doctor + Email FALSE
                                : 6
                            )
                            //Previsión TRUE Doctor + Email + Phone FALSE
                            : 5
                    )
                    //Previsión + Phone TRUE Doctor + Email + Phone FALSE
                    : 0;
            return PageName[index];
        }
    }
}
