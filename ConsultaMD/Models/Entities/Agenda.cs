using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Agenda
    {
        public int Id { get; set; }
        public int DoctorLocationId { get; set; }
        public virtual DoctorMedicalAttention DoctorMedicalAttention { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool OverTime { get; set; }
        public Color Color { get; set; }
    }
}
