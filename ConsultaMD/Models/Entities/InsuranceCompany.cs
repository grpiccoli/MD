using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class InsuranceCompany : Company
    {
        public Uri PatientUri { get; set; }
        public Uri ProviderUri { get; set; }
    }
}
