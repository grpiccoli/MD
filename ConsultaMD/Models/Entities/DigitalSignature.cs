using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class DigitalSignature
    {
        public int Id { get; set; }
        public string PathToKey { get; set; }
    }
}
