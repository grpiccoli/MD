using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Company : Person
    {
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public virtual ICollection<CommercialActivity> CommercialActivities { get; } = new List<CommercialActivity>();
    }
}
