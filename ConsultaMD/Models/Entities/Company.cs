using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Company : Person
    {
        public string NombreFantasia { get; set; }
        public virtual ICollection<CommercialActivity> CommercialActivities { get; set; }
    }
}
