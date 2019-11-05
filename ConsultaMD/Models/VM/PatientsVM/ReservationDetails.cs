using ConsultaMD.Extensions;
using ConsultaMD.Extensions.Validation;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using static ConsultaMD.Data.EspecialidadesData;

namespace ConsultaMD.Models.VM
{
    public class ReservationDetails
    {
        public ReservationDetails(Reservation reservation) {
            Reservation = reservation;
        }
        [BindProperty]
        public Reservation Reservation { get; set; }
        [Display(Name = "Detalles del paciente")]
        [BindProperty]
        public PatientDetails PatientDetails 
        {
            get
            {
                return new PatientDetails(Reservation.Patient);
            }
            private set
            {
                PatientDetails = value;
            }
        }
        [Display(Name = "Detalles del profesional médico")]
        [BindProperty]
        public DoctorDetails DoctorDetails 
        {
            get
            {
                return new DoctorDetails(Reservation.TimeSlot.Agenda.MediumDoctor.Doctor);
            }
            private set
            {
                DoctorDetails = value;
            }
        }
        [Display(Name = "Detalles del lugar de atención")]
        [BindProperty]
        public PlaceDetails PlaceDetails 
        {
            get
            {
                return new PlaceDetails(Reservation.TimeSlot.Agenda.MediumDoctor.MedicalAttentionMedium.Place);
            }
            private set
            {
                PlaceDetails = value;
            }
        }
        [Display(Name = "Detalles de fecha y hora")]
        [BindProperty]
        public TimeSlotDetails TimeSlotDetails 
        {
            get
            {
                return new TimeSlotDetails(Reservation.TimeSlot);
            }
            private set
            {
                TimeSlotDetails = value;
            }
        }
        [Display(Name = "Detalles de pago")]
        [BindProperty]
        public PaymentDetails PaymentDetails { get; private set; }
    }
    public class PatientDetails
    {
        public PatientDetails(Patient patient) {
            Id = patient?.NaturalId;
            Name = patient.Natural.FullNameFirst;
        }
        public int? Id { get; private set; }

        [Display(Name = "Nombre")]
        public string Name { get; private set; }
        [Display(Name = "RUT")]
        public string Rut {
            get
            {
                return RUT.Format(Id.Value);
            }
            private set
            {
                Rut = value;
            }
        }
    }
    public class DoctorDetails
    {
        public DoctorDetails(Doctor doctor)
        {
            Id = doctor?.Id;
            Specialty = doctor.Specialty;
            NaturalId = doctor.NaturalId;
            NaturalName = doctor.Natural.FullNameFirst;
        }
        public int? Id { get; private set;  }
        public bool Sex { get; private set; }
        public Especialidad? Specialty { get; private set; }
        [Display(Name = "Especialidad")]
        public string SpecialtyName
        {
            get
            {
                return Specialty.GetAttrName();
            }
            private set
            {
                SpecialtyName = value;
            }
        }
        public int NaturalId { get; private set; }
        [Display(Name = "RUT")]
        public string Rut
        {
            get
            {
                return RUT.Format(NaturalId);
            }
            private set
            {
                Rut = value;
            }
        }
        [Display(Name = "Nombre")]
        public string NaturalName { get; private set; }
        public string Title
        {
            get
            {
                return Specialty.HasValue ? $"Dr{(Sex ? "" : "a")}." : null;
            }
            private set
            {
                Title = value;
            }
        }
        public string TitleName
        {
            get
            {
                return Title == null ? NaturalName : $"{Title} {NaturalName}";
            }
            private set
            {
                Title = value;
            }
        }
    }
    public class PlaceDetails
    {
        public PlaceDetails(Place place)
        {
            Id = place?.Id;
            Name = place.Name;
            Address = place.Address;
        }
        public string Id { get; private set; }
        public string Name { get; private set; }
        [Display(Name = "Direccion")]
        public string Address { get; private set; }
    }
    public class TimeSlotDetails
    {
        public TimeSlotDetails(TimeSlot timeSlot)
        {
            Id = timeSlot?.Id;
            DateTime = timeSlot.StartTime.ToString("dddd dd MMMM yyyy, h:mm tt", new CultureInfo("es-CL"));
        }
        public int? Id { get; private set; }
        [Display(Name = "Hora")]
        public string DateTime { get; private set; }
    }
    public class PaymentDetails
    {
        [Required]
        [RUT(ErrorMessage = "RUT no válido")]
        [RegularExpression(@"[0-9\.]{7,10}-[0-9Kk]")]
        [Display(Name = "Ingrese RUT Titular de Tarjeta de Pago")]
        public string Rut { get; set; }
        public int Type { get; private set; }
    }
}
