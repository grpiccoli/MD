using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Models.Entities
{
    public class InsuranceLocation
    {
        public int Id { get; set; }
        public int MediumDoctorId { get; set; }
        public virtual MediumDoctor MediumDoctor { get; set; }
        public Insurance Insurance { get; set; }
        public string Password { get; set; }
    }
}
