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
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Ubicación")]
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Display(Name = "Duración de Citas")]
        [DisplayFormat(DataFormatString = "{0:mm}")]
        public TimeSpan Duration { get; set; }
        public IEnumerable<TimeSlot> TimeSlots { get; set; }
        public IEnumerable<EventDayWeek> EventDayWeeks { get; set; }
        [Display(Name = "Fechas")]
        public string DateRange()
        {
            return GetRange("dddd dd MMMM yyyy");
        }
        [Display(Name = "Horario")]
        public string TimeRange()
        {
            return GetRange("hh:mm tt");
        }
        public string GetRange(string format)
        {
            return $"{StartTime.ToString(format, new CultureInfo("es-CL"))} - {EndTime.ToString(format, new CultureInfo("es-CL"))}";
        }
        [Display(Name = "Citas")]
        public string GetSlots()
        {
            return $"Totales: {TimeSlotCount}, Agendadas: {TakenTimeSlotCount}";
        }
        [Display(Name = "Dias")]
        public string GetDays()
        {
            var culture = new CultureInfo("es-CL");
            return string.Join(",", EventDayWeeks
                .Select(e => culture.DateTimeFormat
                .GetDayName(e.DayOfWeek)));
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
