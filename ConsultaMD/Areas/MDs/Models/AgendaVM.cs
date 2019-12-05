using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AgendaVM
    {
        public string DoctorId { get; set; }
        public IEnumerable<SelectListItem> DoctorList { get; set; }
        public IEnumerable<Location> LocationList { get; set; }
        public IEnumerable<string> ColorList { get; set; }
    }
}
