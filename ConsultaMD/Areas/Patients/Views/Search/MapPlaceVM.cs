using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class MapPlaceVM
    {
        public string placeId { get; set; }
        public IEnumerable<MapItemVM> items { get; set; }
    }
}
