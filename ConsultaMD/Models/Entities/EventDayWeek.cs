using System;

namespace ConsultaMD.Models.Entities
{
    public class EventDayWeek
    {
        public int Id { get; set; }
        public int AgendaEventId { get; set; }
        public virtual AgendaEvent AgendaEvent { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
