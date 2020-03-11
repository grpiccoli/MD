using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Models.VM
{
    public class DataPagador
    {
        public int Estado { get; set; }
        public string ExtApellidoPat { get; set; }
        public string ExtApellidoMat { get; set; }
        public string ExtNombres { get; set; }
        public string ExtSexo { get; set; }
        public string ExtFechaNacimi { get; set; }
        public string ExtNomCotizante { get; set; }
        public string ExtGrupoIng { get; set; }
    }
    public class DataBenef : DataPagador
    {
        public string FechaNacimi { get; set; }
        public int EdadBeneficiario { get; set; }
    }
    public class Fonasa : DataBenef
    {
        public string Nombre
        {
            get
            {
                return $"{ExtNombres} {ExtApellidoPat} {ExtApellidoMat}";
            }
            private set
            {
                Nombre = value;
            }
        }
        public List<DocFonasa> Docs { get; } = new List<DocFonasa>();
    }
    public class DocFonasa
    {
        public string Address { get; set; }
        public string Commune { get; set; }
        public string Region { get; set; }
        public int NivelPrestador { get; set; }
        public int CodEspeciali { get; set; }
        public string PrestacionId { get; set; }
        public string RutTratante { get; set; }
        public string RutPrestador { get; set; }
        public string NomTratante { get; set; }
        public string NomPrestador { get; set; }
        public Prestacion Prestacion { get; set; }
    }
    public class PaymentData : DocFonasa
    {
        public string RutBeneficiario { get; set; }
        public string CelularNotification { get; set; }
        public string EmailNotification { get; set; }
        public string PayRut { get; set; }
        public DataBenef DataBenef { get; set; }
    }
    public class ValidaCAT
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public Datos Datos { get; set; }
    }
    public class Datos
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
    }
}
