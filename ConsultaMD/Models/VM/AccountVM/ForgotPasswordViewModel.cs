using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
