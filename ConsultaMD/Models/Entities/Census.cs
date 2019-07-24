using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Census
    {
        public int Id { get; set; }
        public DateTime Year { get; set; }
        public int Count { get; set; }
        public int LocationId { get; set; }
        public virtual Locality Locality { get; set; }
    }
}
