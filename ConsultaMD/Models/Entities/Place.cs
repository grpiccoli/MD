using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaMD.Models.Entities
{
    public class Place : Coordinate
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Address { get; set; }
        public int CommuneId { get; set; }
        public virtual Commune Commune { get; set; }
        public string Name { get; set; }
        //https://maps.google.com/?cid=
        public string CId { get; set; }
        public string PhotoId { get; set; }
    }
}
