using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Province : Locality
    {
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Commune> Communes { get; set; }
        public virtual ICollection<AreaCodeProvince> AreaCodeProvinces { get; set; }
    }
}
