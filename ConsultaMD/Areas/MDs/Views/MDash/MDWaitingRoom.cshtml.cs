using ConsultaMD.Extensions;
using System;
using System.Collections.Generic;
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
        public bool IsConfirmed { get; set; }
        public bool IsIn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class MDWaitingRoomVM
    {
        public MDWaitingRoomPatientVM Attending { get; set; }
        public IEnumerable<MDWaitingRoomPatientVM> Waiting { get; set; }
        public IEnumerable<MDWaitingRoomPatientVM> Confirmed { get; set; }
    }
}
