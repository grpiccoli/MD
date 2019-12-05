using System;
using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class Agenda
    {
        public int Id { get; set; }
        public int AgendaEventId { get; set; }
        public virtual AgendaEvent AgendaEvent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public virtual ICollection<TimeSlot> TimeSlots { get; } = new List<TimeSlot>();
    }
}
