using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Comuna : Locality
    {
        public int ProvinciaId { get; set; }
        public int ElectoralDistrict { get; set; }
        public int SenatorialCircunscription { get; set; }
    }
}
