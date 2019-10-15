using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Models.Entities
{
    public class AreaCode
    {
        [Display(Name = "Código de Área Telefónico")]
        [InsertOff]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public virtual ICollection<AreaCodeProvince> AreaCodeProvinces { get; set; }
    }
}
