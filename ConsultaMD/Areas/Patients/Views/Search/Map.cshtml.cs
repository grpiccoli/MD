using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class MapDocVM
    {
        public int run { get; set; }
        public string dr { get; set; }
        public string especialidad { get; set; }
        public int experience { get; set; }
    }

    public class MapItemVM
    {
        public int run { get; set; }
        public int price { get; set; }
    }

    public class MapPlaceVM
    {
        public string placeId { get; set; }
        public IEnumerable<MapItemVM> items { get; set; }
    }

    public class MapVM
    {
        public int Insurance { get; set; } = 0;
        [Display(Name = "Ubicación")]
        public HashSet<int> Ubicacion { get; } = new HashSet<int>();
        [Display(Name = "Especialidad")]
        public HashSet<int> Especialidad { get; }
            = new HashSet<int>();
        [Display(Name = "Género")]
        public HashSet<bool> Sex { get; } = new HashSet<bool>();
        //[Range(1, 24)]
        public TimeSpan? MinTime { get; set; }
        //[Range(1, 24)]
        public TimeSpan? MaxTime { get; set; }
        public HashSet<DateTime> Dates { get; } = new HashSet<DateTime>();
        //public DateTime? MinDate { get; set; }
        //public DateTime? MaxDate { get; set; }
        public DateTime Last { get; set; }
        //public bool Monday { get; set; } = true;
        //public bool Tuesday { get; set; } = true;
        //public bool Wednesday { get; set; } = true;
        //public bool Thursday { get; set; } = true;
        //public bool Friday { get; set; } = true;
        //public bool Saturday { get; set; } = true;
        //public bool Sunday { get; set; } = true;
        //public HashSet<DayOfWeek> Days {
        //    get
        //    {
        //        var result = new HashSet<DayOfWeek>();
        //        if (!Monday) result.Add(DayOfWeek.Monday);
        //        if (!Tuesday) result.Add(DayOfWeek.Tuesday);
        //        if (!Wednesday) result.Add(DayOfWeek.Wednesday);
        //        if (!Thursday) result.Add(DayOfWeek.Thursday);
        //        if (!Friday) result.Add(DayOfWeek.Friday);
        //        if (!Saturday) result.Add(DayOfWeek.Saturday);
        //        if (!Sunday) result.Add(DayOfWeek.Sunday);
        //        return result;
        //    }
        //    private set
        //    {
        //        Days = value;
        //    }
        //}
        [Display(Name = "Destacar servicios compatibles con plan de salud")]
        public bool HighlightInsurance { get; set; } = true;
    }

    public class PlaceVM
    {
        public string Id { get; set; }
        public string Address { get; set; }
        //https://maps.google.com/?cid=
        public string CId { get; set; }
        public string PhotoId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
