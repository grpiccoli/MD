using ConsultaMD.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ConsultaMD.Models.Entities
{
    public class Doctor
    {
        public Doctor() { }
        public Doctor(SuperData superData) 
        {
            if(superData != null)
            {
                Id = superData.Id;
                RegistryDate = superData.Registry;
                SisId = superData.Sis;
                Specialties = superData.GetSpecialties.ToList();
                YearTitle = superData.TitleDate.Year;
            }
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registro Médico")]
        public int Id { get; set; }
        public int NaturalId { get; set; }
        public Natural Natural { get; set; }
        [DataType(DataType.Date)]
        public DateTime RegistryDate { get; set; }
        //public virtual ICollection<Title> Titles { get; } = new List<Title>();
        public virtual ICollection<DoctorSpecialty> Specialties { get; } = new List<DoctorSpecialty>();
        //public Especialidad? Specialty { get; set; }
        //public string Title { get; set; }
        //public string Institution { get; set; }
        //0 female 1 male
        public string SisId { get; set; }
        public int YearTitle { get; set; }
        public int? FonasaLevel { get; set; }
        //public int YearSpecialty { get; set; }
        //public virtual ICollection<Subspecialty> Subspecialties { get; } = new List<Subspecialty>();
        //public virtual ICollection<Publication> Publications { get; } = new List<Publication>();
        public virtual ICollection<MediumDoctor> MediumDoctors { get; } = new List<MediumDoctor>();
        public int GetYearsExperience()
        {
            return DateTime.Today.Year - YearTitle;
            //return DateTime.Today.Year - Titles.Min(t => t.Date).Year;
        }
        public int? MedicalAttentionId { get; set; }
        public MedicalAttention MedicalAttention { get; set; }
    }
}
