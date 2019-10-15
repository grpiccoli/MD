using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.Entities
{
    public class Vertex : Coordinate
    {
        public int Id { get; set; }
        public int PolygonId { get; set; }
        public virtual Polygon Polygon { get; set; }
        [Display(Name = "Vértice")]
        [Range(1, 200)]
        public int Order { get; set; }
    }
}
