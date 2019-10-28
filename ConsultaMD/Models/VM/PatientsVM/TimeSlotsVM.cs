using ConsultaMD.Models.Entities;
using System;
using System.Globalization;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class TimeSlotVM
    {
        public TimeSlotVM(TimeSlot timeSlot) {
            Id = timeSlot?.Id;
            StartTime = timeSlot?.StartTime;
        }
        public int? Id { get; set; }
        public DateTime? StartTime { get; set; }
        public string Hora
        {
            get
            {
                return StartTime.Value.ToString("dddd dd MMM yyyy<br>HH:mm tt", new CultureInfo("es-CL"));
            }
            private set
            {
                Hora = value;
            }
        }
    }
    public class TimeSlotsVM {
        public int Id { get; set; }
        public string StartTime { get; set; }
    }
}
