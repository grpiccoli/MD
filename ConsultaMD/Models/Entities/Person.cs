using ConsultaMD.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaMD.Models.Entities
{
    public abstract class Person
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "RUT")]
        public int Id { get; set; }
        public string Discriminator { get; set; }
        public string BanmedicaName { get; set; }
        public ICollection<InsuranceLocation> InsuranceLocations { get; } = new List<InsuranceLocation>();
        public string GetRUT()
        {
            return RUT.Format(Id);
        }
        public string GetDV() {
            return RUT.DV(Id);
        }
    }
}
