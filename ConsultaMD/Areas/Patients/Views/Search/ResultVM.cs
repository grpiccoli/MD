using System;
using System.Collections.Generic;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class ResultVM
    {
        public int Run { get; set; }
        public int Price { get; set; }
        public string Dr { get; set; }
        public string Office { get; set; }
        public string Especialidad { get; set; }
        public int Experience { get; set; }
        public bool Sex { get; set; }
        public IEnumerable<Insurance> Insurances { get; set; }
        public DateTime? Next { get; set; }
        public bool Match { get; set; }
    }
}
