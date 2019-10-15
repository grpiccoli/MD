using ConsultaMD.Extensions;
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
        public string FullNameFirst { get; set; }
        public string FullLastFirst { get; set; }
        public string GetRUT()
        {
            return RUT.Format(Id);
        }
    }
}
