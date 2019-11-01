namespace ConsultaMD.Models.Entities
{
    public class MedicalCoverage
    {
        public int QuoteeId { get; set; }
        public virtual Patient Quotee { get; set; }
        public int DependantId { get; set; }
        public virtual Natural Dependant { get; set; }
    }
}
