using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Data
{
    public class InsuranceData
    {
        public enum Insurance
        {
            [Display(Name= "Particular")]
            Particular = 0,
            [Display(Name = "FONASA")]
            Fonasa = 1,
            [Display(Name = "Banmédica")]
            Banmedica = 2,
            [Display(Name = "Colmena")]
            Colmena = 3,
            [Display(Name = "Consalud")]
            Consalud = 4,
            [Display(Name = "CruzBlanca")]
            CruzBlanca = 5,
            [Display(Name = "Nueva Masvida")]
            NvaVida = 6,
            [Display(Name = "Vida Tres")]
            Vida3 = 7
        }
    }
}
