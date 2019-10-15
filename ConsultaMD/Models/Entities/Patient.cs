using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Models.Entities
{
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NaturalId { get; set; }
        public Natural Natural { get; set; }
        public Insurance Insurance { get; set; }
        public string InsurancePassword { get; set; }
        public virtual ICollection<Dependency> Dependants { get; set; }
        public virtual ICollection<TimeSlot> TimeSlotAppointments { get; set; }
    }
}
