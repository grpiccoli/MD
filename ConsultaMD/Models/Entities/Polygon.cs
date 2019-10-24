using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Polygon
    {
        public int Id { get; set; }
        public int LocalityId { get; set; }
        public virtual Locality Locality { get; set; }
        public virtual ICollection<Vertex> Vertices { get; } = new List<Vertex>();
    }
}
