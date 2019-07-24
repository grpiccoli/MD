using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class UserInsurance
    {
        public int PatitientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int InsuranceCompanyId { get; set; }
        public virtual InsuranceCompany InsuranceCompany { get; set; }
        public string Password { get; set; }
    }
}
