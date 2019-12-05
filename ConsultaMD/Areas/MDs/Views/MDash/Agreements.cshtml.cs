using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AgreementVM
    {
        public int Id { get; set; }
        public Insurance Insurance { get; set; }
        public string PersonRUT { get; set; }
        public string PersonName { get; set; }
        [Display(Name = "Ubicaciones Inactivas")]
        public IEnumerable<string> InactiveLocations { get; set; }
        [Display(Name = "Ubicaciones Activas")]
        public IEnumerable<string> ActiveLocations { get; set; }
    }
}
