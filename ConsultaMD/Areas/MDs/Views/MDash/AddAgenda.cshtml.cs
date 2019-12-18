using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.MDs.Models
{
    public class AddAgendaVM
    {
        public string SelectorList { get; set; }
        public int MediumDoctorId { get; set; }
        [Display(Name = "Inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:ddd dd MMMM yyyy, h:mm tt}")]
        public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);
        [Display(Name = "Término")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:ddd dd MMMM yyyy, h:mm tt}")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(2);
        [Display(Name = "Duración (minutos)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:mm}")]
        public int Duration { get; set; } = 10;
        [Display(Name = "¿Tiene sobre cupos?")]
        public bool HasOverTime { get; set; } = true;
        [Display(Name = "¿Cada cuantas semanas? (semanas)")]
        [Range(1, 10)]
        public int Frequency { get; set; } = 1;
        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } = true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = true;
        public bool Sunday { get; set; } = true;
        public HashSet<DayOfWeek> DaysOfWeek
        {
            get
            {
                var result = new HashSet<DayOfWeek>();
                if (Monday) result.Add(DayOfWeek.Monday);
                if (Tuesday) result.Add(DayOfWeek.Tuesday);
                if (Wednesday) result.Add(DayOfWeek.Wednesday);
                if (Thursday) result.Add(DayOfWeek.Thursday);
                if (Friday) result.Add(DayOfWeek.Friday);
                if (Saturday) result.Add(DayOfWeek.Saturday);
                if (Sunday) result.Add(DayOfWeek.Sunday);
                return  result;
            }
            private set
            {
                DaysOfWeek = value;
            }
        }
    }

}
