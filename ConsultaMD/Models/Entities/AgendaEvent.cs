using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaMD.Models.Entities
{
    public class AgendaEvent
    {
        public int Id { get; set; }
        public int MediumDoctorId { get; set; }
        public virtual MediumDoctor MediumDoctor { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int Frequency { get; set; }
        //public RepeatEvery RepeatEvery { get; set; }
        [NotMapped]
        public List<DayOfWeek> daysOfWeek { get; set; }
        public ICollection<EventDayWeek> EventDayWeeks { get; } = new List<EventDayWeek>();
        public virtual ICollection<Agenda> Agendas { get; } = new List<Agenda>();
    }
    //public enum RepeatEvery
    //{
        //[Display(Name = "Diariamente")]
        //Daily = 1,
        //[Display(Name = "Semanalmente")]
        //Weekly = 7,
        //[Display(Name = "Mensualmente")]
        //Monthly = 30
        //[Display(Name = "Anualmente")]
        //Yearly = 360
    //}
}
