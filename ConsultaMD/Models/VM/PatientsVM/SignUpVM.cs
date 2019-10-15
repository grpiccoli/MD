using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class SingUpVM
    {
        [Required]
        public string RUT { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Previsión")]
        public string Insurance { get; set; }
        [Required]
        [Display(Name="Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo")]
        public string EMail { get; set; }
    }
}
