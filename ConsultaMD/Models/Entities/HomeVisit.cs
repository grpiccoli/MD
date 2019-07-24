using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class HomeVisit : MedicalAttention
    {
        public int ComunaId { get; set; }
        public virtual Comuna Comuna { get; set; }
    }
}
