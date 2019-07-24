using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class MedicalAttention
    {
        public int Id { get; set; }
        public List<DoctorMedicalAttention> DoctorMedicalAttentions { get; set; }
    }
}
