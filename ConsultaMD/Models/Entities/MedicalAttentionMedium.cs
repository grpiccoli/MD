using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public abstract class MedicalAttentionMedium
    {
        public int Id { get; set; }
        public string Discriminator { get; set; }
        public virtual ICollection<MediumDoctor> MediumDoctors { get; } = new List<MediumDoctor>();
    }
}
