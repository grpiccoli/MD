using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Specialty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DoctorSpecialty> Doctors { get; } = new List<DoctorSpecialty>();
    }
}
