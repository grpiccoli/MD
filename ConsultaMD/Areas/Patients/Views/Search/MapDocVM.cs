using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ConsultaMD.Data.EspecialidadesData;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class MapDocVM
    {
        public int run { get; set; }
        public string dr { get; set; }
        public string especialidad { get; set; }
        public int experience { get; set; }
    }
}
