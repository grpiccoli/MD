using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public abstract class MedicalAttentionMedium
    {
        public int Id { get; set; }
        public string Discriminator { get; set; }
        public string PlaceId { get; set; }
        public virtual Place Place { get; set; }
        public virtual ICollection<MediumDoctor> MediumDoctors { get; } = new List<MediumDoctor>();
    }
}
