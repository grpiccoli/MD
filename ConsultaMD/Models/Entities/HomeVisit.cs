namespace ConsultaMD.Models.Entities
{
    public class HomeVisit : MedicalAttentionMedium
    {
        public int? CommuneId { get; set; }
        public virtual Commune Commune { get; set; }
    }
}
