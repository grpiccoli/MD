using System;
using System.ComponentModel.DataAnnotations;
using static ConsultaMD.Data.EspecialidadesData;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class PaymentVM
    {
        [Display(Prompt = "RUT")]
        public string RutCliente { get; set; }

        [Display(Prompt = "Nombre")]
        public string NombreCliente { get; set; }

        [Display(Prompt = "Dia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy}")]
        public string Dia { get; set; }

        [Display(Prompt = "Hora")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:H:mm}")]
        public string Hora { get; set; }

        [Display(Prompt = "Direccion")]
        public string Direccion { get; set; }

        [Display(Prompt = "RUT")]
        public string RutDoctor { get; set; }
        public int DoctorId { get; set; }

        [Display(Prompt = "Nombre")]
        public string NombreDoctor { get; set; }

        [Display(Prompt = "Especialidad")]
        public string Especialidad { get; set; }
        public Especialidades? EspecialidadId { get; set; }
        public string Lugar { get; set; }
        public string PlaceId { get; set; }
        public string area { get; set; }
        public int PaymentType { get; set; }
    }
}
