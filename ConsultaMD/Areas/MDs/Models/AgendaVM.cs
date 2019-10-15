using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AgendaVM
    {
        public string DoctorId { get; set; }
        public IEnumerable<SelectListItem> DoctorList { get; set; }
        public IEnumerable<Location> LocationList { get; set; }
        public string[] ColorList { get; set; }
    }
}
