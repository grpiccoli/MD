using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class CreditCard
    {
        public int CardNumber { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string Name { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CVV { get; set; }
        public string Address { get; set; }
    }
}
