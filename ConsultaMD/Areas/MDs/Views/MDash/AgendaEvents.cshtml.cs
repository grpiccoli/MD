using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AgendaEventVM
    {
        public int Id { get; set; }
        [Display(Name = "Ubicación")]
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Display(Name = "Duración de Citas")]
        public TimeSpan Duration { get; set; }
        public IEnumerable<TimeSlot> TimeSlots { get; set; }
        public IEnumerable<EventDayWeek> EventDayWeeks { get; set; }
        [Display(Name = "Fechas")]
        public string DateRange()
        {
            return GetRange("{0:dd/MM/yyyy}");
        }
        [Display(Name = "Horario")]
        public string TimeRange()
        {
            return GetRange("{0:hh:mm tt}");
        }
        public string GetRange(string format)
        {
            return $"{StartTime.ToString(format, CultureInfo.InvariantCulture)} - {EndTime.ToString(format, CultureInfo.InvariantCulture)}";
        }
        [Display(Name = "Citas")]
        public string GetSlots()
        {
            return $"Totales: {TimeSlotCount}, Agendadas: {TakenTimeSlotCount}";
        }
        [Display(Name = "Dias")]
        public string GetDays()
        {
            return string.Join(",", EventDayWeeks.Select(e => e.DayOfWeek.ToString()));
        }
        //[Display(Name = "Citas Totales")]
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
        //[Display(Name = "Citas Agendadas")]
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
}
