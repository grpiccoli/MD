using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
