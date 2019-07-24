using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class DoctorMedicalAttention
    {
        public int MedicalAttentionId { get; set; }
        public virtual MedicalAttention MedicalAttention { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
