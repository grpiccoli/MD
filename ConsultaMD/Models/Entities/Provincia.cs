using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Provincia : Locality
    {
        public int RegionId { get; set; }
        public List<Comuna> Comunas { get; set; }
        public List<AreaCodeProvincia> AreaCodeProvincias { get; set; }
    }
}
