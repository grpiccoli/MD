using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Patient : Natural
    {
        public int UserInsuranceId { get; set; }
        public virtual UserInsurance UserInsurance { get; set; }
        public string Password { get; set; }
        public int MobilePhone { get; set; }
        public string Email { get; set; }
        public List<Natural> Dependants { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
