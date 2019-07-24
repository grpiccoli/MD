using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.VM
{
    public class LoginVM
    {
        [Required]
        [Display(Prompt = "RUT")]
        public string RUT { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Prompt = "Contraseña")]
        public string Password { get; set; }
    }
}
