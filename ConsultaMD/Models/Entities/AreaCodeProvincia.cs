using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class AreaCodeProvincia
    {
        public int ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }
        public int AreaCodeId { get; set; }
        public virtual AreaCode AreaCode { get; set; }
    }
}
