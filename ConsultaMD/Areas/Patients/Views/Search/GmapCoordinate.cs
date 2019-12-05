using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class GmapCoordinate
    {
        [Range(-90, -17)]
        public double lat { get; set; }
        [Range(-110, -60)]
        public double lng { get; set; }
    }
}
