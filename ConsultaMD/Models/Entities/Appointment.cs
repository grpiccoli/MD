using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int TimeSlotId { get; set; }
        public int PatientId { get; set; }
    }
}
