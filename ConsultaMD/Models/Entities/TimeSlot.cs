using System;

namespace ConsultaMD.Models.Entities
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public int AgendaId { get; set; }
        public virtual Agenda Agenda { get; set; }
        public int? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public bool Taken { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
