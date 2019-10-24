namespace ConsultaMD.Models.Entities
{
    public class MedicalCoverage
    {
        public int BeneficiaryId { get; set; }
        public virtual Patient Beneficiary { get; set; }
        public int DependantId { get; set; }
        public virtual Natural Dependant { get; set; }
    }
}
