using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Dependency
    {
        public int BeneficiaryId { get; set; }
        public virtual Patient Beneficiary { get; set; }
        public int DependantId { get; set; }
        public virtual Natural Dependant { get; set; }
    }
}
