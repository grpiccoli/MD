using ConsultaMD.Models.Entities;

namespace ConsultaMD.Models.VM
{
    public class Fonasa
    {
        public int Estado { get; set; }
        public string ExtApellidoPat { get; set; }
        public string ExtApellidoMat { get; set; }
        public string ExtNombres { get; set; }
        public string ExtSexo { get; set; }
        public string ExtFechaNacimi { get; set; }
        public string ExtNomCotizante { get; set; }
        public string ExtGrupoIng { get; set; }
        public string FechaNacimi { get; set; }
        public int EdadBeneficiario { get; set; }
        public string BrowserWSEndpoint { get; set; }
        public string Text { get; set; }
    }
    public class DocFonasa
    {
        public string Address { get; set; }
        public string Commune { get; set; }
        public string Region { get; set; }
        public int Nivel { get; set; }
        public string PrestacionId { get; set; }
        public string RutTratante { get; set; }
        public string NomTratante { get; set; }
        public Prestacion Prestacion { get; set; }
    }
}
