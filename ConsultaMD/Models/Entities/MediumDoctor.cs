using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class MediumDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int? MedicalAttentionMediumId { get; set; }
        public virtual MedicalAttentionMedium MedicalAttentionMedium { get; set; }
        public int PriceParticular { get; set; }
        public bool OverTime { get; set; }
        public string Color { get; set; }
        public virtual ICollection<InsuranceLocation> InsuranceLocations { get; } = new List<InsuranceLocation>();
        public virtual ICollection<AgendaEvent> AgendaEvents { get; } = new List<AgendaEvent>();
    }
}
