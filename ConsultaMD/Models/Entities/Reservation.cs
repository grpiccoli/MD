using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
        public int? BondId { get; set; }
    }
}
