using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Prestacion
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Total { get; set; }
        public int Copago { get; set; }
        public virtual ICollection<InsuranceLocation> InsuranceLocation { get; } = new List<InsuranceLocation>();
    }
}
