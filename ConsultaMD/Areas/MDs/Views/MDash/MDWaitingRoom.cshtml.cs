using ConsultaMD.Extensions;
using System;
using static ConsultaMD.Data.InsuranceData;

namespace ConsultaMD.Areas.MDs.Models
{
    public class MDWaitingRoomPatientVM
    {
        public int Id { get; set; }
        public string GetRUT()
        {
            return RUT.Format(Id);
        }
        public string Name { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public Insurance Insurance { get; set; }
        public bool IsInLine { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
