using System;

namespace ConsultaMD.Models.Entities
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public int AgendaId { get; set; }
        public virtual Agenda Agenda { get; set; }
        public int? ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
