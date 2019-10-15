using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class AreaCodeProvince
    {
        [InsertOff]
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        [InsertOff]
        public int AreaCodeId { get; set; }
        public virtual AreaCode AreaCode { get; set; }
    }
}
