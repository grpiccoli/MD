using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class MedicalOffice : MedicalAttention
    {
        public int OfficeAddressId { get; set; }
        public virtual Address Office { get; set; }
        public string ParkingAddressId { get; set; }
        public virtual Address Parking { get; set; }
        public int ComunaId { get; set; }
        public virtual Comuna Comuna { get; set; }
        public List<string> Photos { get; set; }
    }
}
