using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.MDs.Models
{
    public class LocationVM
    {
        [Display(Name = "Dirección")]
        public string Address { get; set; }
        public string Name { get; set; }
        [Display(Name = "Convenios")]
        public IEnumerable<string> Agreements { get; set; }
        public string Commune { get; set; }
        public string PlaceId { get; set; }
        public string PhotoId { get; set; }
        public string CId { get; set; }
    }
}
