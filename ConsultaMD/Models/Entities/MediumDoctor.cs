using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaMD.Models.Entities
{
    public class MediumDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int MedicalAttentionMediumId { get; set; }
        public MedicalAttentionMedium MedicalAttentionMedium { get; set; }
        public int PriceParticular { get; set; }
        public bool OverTime { get; set; }
        public string Color { get; set; }
        public virtual ICollection<InsuranceLocation> InsuranceLocations { get; } = new List<InsuranceLocation>();
        public virtual ICollection<Agenda> Agendas { get; } = new List<Agenda>();
    }
}
