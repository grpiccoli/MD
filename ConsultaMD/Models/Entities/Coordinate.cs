using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Coordinate
    {
        public int Id { get; set; }
        public int PolygonId { get; set; }
        public virtual Polygon Polygon { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Vertex { get; set; }
    }
}
