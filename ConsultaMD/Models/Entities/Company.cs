using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Company : Person
    {
        public string NombreFantasia { get; set; }
        public List<CommercialActivity> CommercialActivities { get; set; }
    }
}
