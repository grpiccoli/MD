﻿using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class MedicalOffice : MedicalAttentionMedium
    {
        public string Block { get; set; }
        public string Floor { get; set; }
        public string Appartment { get; set; }
        public string Office { get; set; }
    }
}
