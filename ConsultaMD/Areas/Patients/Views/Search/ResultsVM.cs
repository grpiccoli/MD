using System.Collections.Generic;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class ResultsVM
    {
        public PlaceVM Place { get; set; }
        public IEnumerable<ResultVM> Items { get; set; }
    }
}
