using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.VM.PatientsVM
{
    public class DependentVM
    {
        [Display(Prompt = "RUT")]
        public string RutCliente { get; set; }

        [Display(Prompt = "Nombre")]
        public string NombreCliente { get; set; }
    }
}
