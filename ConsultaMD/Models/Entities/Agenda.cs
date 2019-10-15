using System;
using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Agenda
    {
        public int Id { get; set; }
        public int MediumDoctorId { get; set; }
        public virtual MediumDoctor MediumDoctor { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public virtual ICollection<TimeSlot> TimeSlots { get; set; }
    }
}
