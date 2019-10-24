using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.MDs.Models
{
    public class DoctorSelect : SelectListItem
    {
        public override bool Equals(object obj)
        {
            return obj is DoctorSelect q && q.Value == Value;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode(StringComparison.InvariantCulture);
        }
    }
}
