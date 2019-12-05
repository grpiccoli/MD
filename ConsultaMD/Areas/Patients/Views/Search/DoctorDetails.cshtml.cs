using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class DoctorDetailsVM
    {
        public Doctor DocVM { get; set; }
        public int MdId { get; set; }
        public IEnumerable<SelectListItem> MdList { get; set; }
        public int PlaceId { get; set; }
        public IEnumerable<SelectListItem> PlaceList { get; set; }
        public int YearsofExperience { get; set; }
        public int Nregistro { get; set; }
        public string Date { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime? Last { get; set; }
        public string Title { get; set; }
    }
}
