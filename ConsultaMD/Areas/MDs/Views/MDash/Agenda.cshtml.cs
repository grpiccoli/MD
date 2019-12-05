using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AgendaEventVM
    {
        [Display(Name = "Ubicación")]
        public string Location { get; set; }
        [Display(Name = "Hora de Inicio")]
        public TimeSpan StartTime { get; set; }
        [Display(Name = "Hora de Término")]
        public TimeSpan EndTime { get; set; }
        [Display(Name = "Duración de Citas")]
        public TimeSpan Duration { get; set; }
        public IEnumerable<TimeSlot> TimeSlots { get; set; }
        [Display(Name = "Citas Totales")]
        public int TimeSlotCount 
        {
            get
            {
                return TimeSlots.Count();
            }
            private set
            {
                TimeSlotCount = value;
            }
        }
        [Display(Name = "Citas Agendadas")]
        public int TakenTimeSlotCount 
        {
            get
            {
                return TimeSlots.Where(t => t.ReservationId != 0).Count();
            }
            private set
            {
                TimeSlotCount = value;
            }
        }
    }

    public class AddAgendaVM
    {
        public int MedicalAttentionMediumId { get; set; }
        [Display(Name = "Inicio")]
        public DateTime StartTime { get; set; }
        [Display(Name = "Término")]
        public DateTime EndTime { get; set; }
        [Display(Name = "Duración (minutos)")]
        public TimeSpan Duration { get; set; } = new TimeSpan(0, 10, 0);
        [Display(Name = "¿Tiene sobre cupos?")]
        public bool HasOverTime { get; set; }
        [Display(Name = "Frecuencia (semanas)")]
        public int Frequency { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public HashSet<DayOfWeek> Days
        {
            get
            {
                var result = new HashSet<DayOfWeek>();
                if (!Monday) result.Add(DayOfWeek.Monday);
                if (!Tuesday) result.Add(DayOfWeek.Tuesday);
                if (!Wednesday) result.Add(DayOfWeek.Wednesday);
                if (!Thursday) result.Add(DayOfWeek.Thursday);
                if (!Friday) result.Add(DayOfWeek.Friday);
                if (!Saturday) result.Add(DayOfWeek.Saturday);
                if (!Sunday) result.Add(DayOfWeek.Sunday);
                return result;
            }
            private set
            {
                Days = value;
            }
        }
    }
}
