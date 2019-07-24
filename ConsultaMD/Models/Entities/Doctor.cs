using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class Doctor : Natural
    {
        [Required]
        public int CarnetId { get; set; }
        public virtual Carnet Carnet { get; set; }
        public int CreditCardId { get; set; }
        public virtual CreditCard CreditCard { get; set; }
        public int DigitalSignatureId { get; set; }
        public virtual DigitalSignature DigitalSignature { get; set; }
        public int SuperintendenceId { get; set; }
        public int YearsExperience { get; set; }
        public string Specialty { get; set; }
        public List<string> SubSpecialties { get; set; }
        public List<string> Publications { get; set; }
    }
}
