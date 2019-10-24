using System.Collections.Generic;
using System.Linq;

namespace ConsultaMD.Areas.Patients.Views.Search
{
    public class ResultsVM
    {
        public PlaceVM Place { get; set; }
        public IOrderedEnumerable<ResultVM> Items { get; set; }
    }
}
