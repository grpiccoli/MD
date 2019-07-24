using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class InsuranceLocation
    {
        public int Id { get; set; }
        public int InsuranceCompanyId { get; set; }
        public virtual InsuranceCompany InsuranceCompany { get; set; }
        public int DoctorMedicalAttentionId { get; set; }
        public virtual DoctorMedicalAttention DoctorMedicalAttention { get; set; }
        public string Password { get; set; }
        public int Price { get; set; }
    }
}
