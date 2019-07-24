using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Locality
    {
        public int Id { get; set; }
        public List<Census> Censuses { get; set; }
        public int Surface { get; set; }
        public string Name { get; set; }
        public List<Polygon> Polygons { get; set; }
    }
}
