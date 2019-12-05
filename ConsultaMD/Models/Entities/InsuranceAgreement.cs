using System.Collections.Generic;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Models.Entities
{
    public class InsuranceAgreement
    {
        public int Id { get; set; }
        public Insurance Insurance { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public string Password { get; set; }
        public virtual ICollection<InsuranceLocation> InsuranceLocations { get; } = new List<InsuranceLocation>();
    }
}
