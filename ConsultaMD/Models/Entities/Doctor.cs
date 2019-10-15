using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ConsultaMD.Data.EspecialidadesData;

namespace ConsultaMD.Models.Entities
{
    public class Doctor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registro Médico")]
        public int Id { get; set; }
        public int NaturalId { get; set; }
        public Natural Natural { get; set; }
        public int DigitalSignatureId { get; set; }
        public virtual DigitalSignature DigitalSignature { get; set; }
        [DataType(DataType.Date)]
        public DateTime RegistryDate { get; set; }
        public Especialidades? Specialty { get; set; }
        public string Title { get; set; }
        public string Institution { get; set; }
        //0 female 1 male
        public bool Sex { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public string SisId { get; set; }
        public int YearTitle { get; set; }
        public int YearSpecialty { get; set; }
        public virtual ICollection<Subspecialty> Subspecialties { get; set; }
        public virtual ICollection<Publication> Publications { get; set; }
        public virtual ICollection<MediumDoctor> MediumDoctors { get; set; }
        public int GetYearsExperience()
        {
            return DateTime.Today.Year - YearTitle;
        }
    }
}
