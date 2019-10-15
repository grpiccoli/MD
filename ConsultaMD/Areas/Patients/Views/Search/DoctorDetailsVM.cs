using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class DoctorDetailsVM
    {
        public Doctor DocVM { get; set; }
        public string PlaceId { get; set; }
        public IEnumerable<SelectListItem> PlacesList { get; set; }
        public int YearsofExperience { get; set; }
        public int Nregistro { get; set; }
        public DateTime Date { get; set; }
        public int Time { get; set; }
        public DateTime Last { get; set; }
        public string Title { get; set; }
    }
}
