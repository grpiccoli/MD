namespace ConsultaMD.Models.Entities
{
    public class DoctorSpecialty
    {
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public int SpecialtyId { get; set; }
        public virtual Specialty Specialty { get; set; }
    }
}
