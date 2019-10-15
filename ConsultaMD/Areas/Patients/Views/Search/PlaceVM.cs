using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class PlaceVM
    {
        public string Id { get; set; }
        public string Address { get; set; }
        //https://maps.google.com/?cid=
        public string CId { get; set; }
        public string PhotoId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
