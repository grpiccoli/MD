using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Commune : Locality
    {
        public int? ElectoralDistrict { get; set; }
        public int? SenatorialCircunscription { get; set; }
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        public virtual ICollection<HomeVisit> HomeVisits { get; } = new List<HomeVisit>();
        public virtual ICollection<Place> Places { get; } = new List<Place>();
    }
}
