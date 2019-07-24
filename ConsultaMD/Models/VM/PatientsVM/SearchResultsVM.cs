using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class SearchResult
    {
        public string FullName { get; set; }
        public int YearsExperience { get; set; }
        public IEnumerable<string> SubSpecialties { get; set; }
        public IEnumerable<string> Publications { get; set; }
        public DateTime NextAvailable { get; set; }
        public int Rating { get; set; }
        public string Specialty { get; set; }
        public int Price { get; set; }
    }
}
