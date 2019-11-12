using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Models.VM
{
    public class UserInitializerVM
    {
        public string Names { get; set; }
        public string LastF { get; set; }
        public string LastM { get; set; }
        public string Banmedica { get; set; }

        public string Role { get; set; }

        public string Claim { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }

        public string Image { get; set; }

        public int? Rating { get; set; }
        public int RUN { get; set; }
        public int Carnet { get; set; }
        public Insurance Insurance { get; set; }
        public DateTime Birth { get; set; }
        public bool Sex { get; set; }
        public Tramo Tramo { get; set; }
    }
}
