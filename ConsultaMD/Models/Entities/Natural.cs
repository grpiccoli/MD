using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.Entities
{
    public class Natural : Person
    {
        public int CarnetId { get; set; }
        public virtual Carnet Carnet { get; set; }
        public virtual Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string GetName()
        {
            if(!string.IsNullOrWhiteSpace(FullNameFirst))
                return FullNameFirst.Split(" ")[0];
            return FullLastFirst.Split(" ")[0];
        }
        public string GetSurnameF()
        {
            if (!string.IsNullOrWhiteSpace(FullLastFirst))
                return FullLastFirst.Split(" ")[0];
            return FullNameFirst.Split(" ")[0];
        }
    }
}
