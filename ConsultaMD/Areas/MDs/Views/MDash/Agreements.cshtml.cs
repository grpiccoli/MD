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
        [Display(Name = "Ubicaciones Inactivas Convenio")]
        public IEnumerable<AgreementLocationVM> InactiveLocations { get; set; }
        [Display(Name = "Ubicaciones Activas Convenio")]
        public IEnumerable<AgreementLocationVM> ActiveLocations { get; set; }
    }
    public class AgreementLocationVM
    {
        public string Name { get; set; }
        public int MId { get; set; }
        public int IId { get; set; }
    }
}
