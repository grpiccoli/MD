using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class AreaCode
    {
        public int Id { get; set; }
        public List<AreaCodeProvincia> AreaCodeProvincias { get; set; }
    }
}
