using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.Entities
{
    public abstract class Coordinate
    {
        [Display(Name = "Latitud")]
        [Range(-90, -17)]
        public double Latitude { get; set; }
        [Display(Name = "Longitud")]
        [Range(-110, -60)]
        public double Longitude { get; set; }
    }
}
