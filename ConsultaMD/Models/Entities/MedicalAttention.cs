using System;

namespace ConsultaMD.Models.Entities
{
    public class MedicalAttention
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
