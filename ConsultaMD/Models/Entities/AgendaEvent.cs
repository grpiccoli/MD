using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

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
        public HashSet<DayOfWeek> DaysOfWeek 
        { 
            get 
            {
                var list = Days.ToString(CultureInfo.InvariantCulture)
                    .ToCharArray().Select(a => a - '0').Cast<DayOfWeek>();
                return new HashSet<DayOfWeek>(list);
            } 
        }
        [NotMapped]
        public int Days { get; set; }
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
