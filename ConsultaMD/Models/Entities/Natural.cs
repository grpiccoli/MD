using ConsultaMD.Models.VM;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConsultaMD.Models.Entities
{
    public class Natural : Person
    {
        public void AddFonasa(Fonasa fonasa)
        {
            LastFather = fonasa?.ExtApellidoPat;
            LastMother = fonasa.ExtApellidoMat;
            Names = fonasa.ExtNombres;
            Sex = fonasa.ExtSexo == "M";
            Birth = DateTime.ParseExact(fonasa.ExtFechaNacimi, "YYYYMMDD", CultureInfo.InvariantCulture);
            Patient.Tramo = Enum.Parse<Tramo>(fonasa.ExtGrupoIng);
        }
        public int CarnetId { get; set; }
        public virtual Carnet Carnet { get; set; }
        public virtual Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public int DigitalSignatureId { get; set; }
        public virtual DigitalSignature DigitalSignature { get; set; }
        public string LastFather { get; set; }
        public string LastMother { get; set; }
        public string Names { get; set; }
        public string FullNameFirst { get; set; }
        public string FullLastFirst { get; set; }
        public bool Sex { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public int Age()
        {
            int age = DateTime.Today.Year - Birth.Year;
            if (DateTime.Today < Birth.AddYears(age))
                age--;
            return age;
        }
        public string GetName()
        {
            if(!string.IsNullOrWhiteSpace(FullNameFirst))
                return FullNameFirst.Split(" ")[0];
            return FullLastFirst.Split(" ")[0];
        }
        public string GetSurname()
        {
            if (!string.IsNullOrWhiteSpace(FullLastFirst))
                return FullLastFirst.Split(" ")[0];
            return FullNameFirst.Split(" ")[0];
        }
    }
}
