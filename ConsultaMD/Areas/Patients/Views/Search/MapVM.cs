using ConsultaMD.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ConsultaMD.Data.EspecialidadesData;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class MapVM
    {
        public Insurance Insurance { get; set; } = Insurance.Particular;
        [Display(Name = "Ubicación")]
        public HashSet<int> Ubicacion { get; } = new HashSet<int>();
        [Display(Name = "Especialidad")]
        public HashSet<Especialidad> Especialidad { get; } 
            = new HashSet<Especialidad>();
        [Display(Name = "Género")]
        public HashSet<bool> Sex { get; } = new HashSet<bool>();
        [Range(1, 24)]
        public int? MinTime { get; set; }
        [Range(1, 24)]
        public int? MaxTime { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime Last { get; set; }
        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } = true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = true;
        public bool Sunday { get; set; } = true;
        public HashSet<DayOfWeek> Days {
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
        public bool HighlightInsurance { get; set; } = true;
    }
}
