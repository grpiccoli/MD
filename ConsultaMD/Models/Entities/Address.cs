using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public int PolygonId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Block { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }
        public string Office { get; set; }
    }
}
