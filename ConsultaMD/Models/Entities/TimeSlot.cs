using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public int AgendaId { get; set; }
        public virtual Agenda Agenda { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
