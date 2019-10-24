using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public abstract class Locality
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public virtual ICollection<Census> Censuses { get; } = new List<Census>();
        public int? Surface { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public virtual ICollection<Polygon> Polygons { get; } = new List<Polygon>();
        //public int? ParentLocalityId { get; set; }
        //public virtual Locality ParentLocality { get; set; }
        //public virtual ICollection<Locality> ChildLocalities { get; set; }
    }
}
