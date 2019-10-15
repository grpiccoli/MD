using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
