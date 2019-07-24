using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class DashVM
    {
        public IEnumerable<SearchResult> SearchResults { get; set; }
        public IEnumerable<Locality> Localities { get; set; }
        public string SearchTerm { get; set; }
    }
}
