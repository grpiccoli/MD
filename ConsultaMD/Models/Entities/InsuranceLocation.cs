using ConsultaMD.Extensions;
using System.Globalization;

namespace ConsultaMD.Models.Entities
{
    public class InsuranceLocation
    {
        public int Id { get; set; }
        public int MediumDoctorId { get; set; }
        public virtual MediumDoctor MediumDoctor { get; set; }
        public int InsuranceAgreementId { get; set; }
        public virtual InsuranceAgreement InsuranceAgreement { get; set; }
        public string InsuranceSelector { get; set; }
        public string Address { get; set; }
        public virtual Commune Commune { get; set; }
        public int CommuneId { get; set; }
        public string PrestacionId { get; set; }
        public virtual Prestacion Prestacion { get; set; }
        public string GetName()
        {
            var com = Commune.Name;
            var add = Address;
            //var rut = RUT.Format(InsuranceAgreement.PersonId);
            //var mi = InsuranceAgreement.Insurance.GetAttrName();
            var desc = Prestacion.Description;
            var price = Prestacion.Total.ToString("C", new CultureInfo("es-CL"));
            return $"{add}, {com}, {desc} {price}";
        }
    }
}
