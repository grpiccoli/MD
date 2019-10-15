using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.MDs.Models
{
    public class Cita
    {
            public override bool Equals(object obj)
            {
                return obj is Cita q && q.Id == Id;
            }
            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
            public int Id { get; set; }
            [Display(Name = "Atención")]
            public string Atencion { get; set; }
            public string Title { get; set; }
            [Display(Name = "Lugar")]
            public string Lugar { get; set; }
            public string Nombres { get; set; }
            public string Apellido1 { get; set; }
            public string Apellido2 { get; set; }
            public string BackgroundColor { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Fecha y Hora de Inicio")]
            public DateTime? Start { get; set; }
            [Display(Name = "Fecha y Hora de Término")]
            [DataType(DataType.Text)]
            public DateTime? End { get; set; }
            [Display(Name = "RUN")]
            public int? RUN { get; set; }
            public int? DV { get; set; }
            [Display(Name = "RUT")]
            public string RUT { get; set; }
            public int? DoctorId { get; set; }
            //public string More { get; set; }
            public int? LocId { get; set; }
            [Display(Name = "Edad")]
            public int? Edad { get; set; }
            public string Url { get; set; }
            [Display(Name = "Previsión")]
            public string Prevision { get; set; }
        }
        public enum Prevision
        {
            [Display(Name = "ARMADA")]
            ARMADA = 1,
            [Display(Name = "ACHS")]
            ACHS = 2,
            [Display(Name = "Banmédica")]
            Banmdica = 3,
            [Display(Name = "CAPREDENA")]
            CAPREDENA = 4,
            [Display(Name = "Caso Social")]
            Caso_Social = 5,
            [Display(Name = "Clínica Puerto Montt")]
            Clca_PtoM = 6,
            [Display(Name = "Colmena")]
            Colmena = 7,
            [Display(Name = "Comisión Médica")]
            Comision_Medica = 8,
            [Display(Name = "Consalud")]
            Consalud = 9,
            [Display(Name = "CRUZ BLANCA")]
            Cruz_Blanca = 10,
            [Display(Name = "DIPRECA")]
            Dipreca = 11,
            [Display(Name = "FONASA")]
            Fonasa = 13,
            [Display(Name = "FACH")]
            FACH = 14,
            [Display(Name = "FUNDACIÓN")]
            Fundacion = 15,
            [Display(Name = "GES ISAPRE")]
            Ges_Isapre = 16,
            [Display(Name = "Hospital")]
            Hospital = 17,
            [Display(Name = "IST")]
            IST = 18,
            [Display(Name = "Jeofosale")]
            Jeofosale = 19,
            [Display(Name = "MÁS VIDA")]
            Mas_vida = 20,
            [Display(Name = "Mutual de Seguridad")]
            Mutual_seguridad = 21,
            [Display(Name = "PARTICULAR")]
            Particular = 22,
            [Display(Name = "PRAIS")]
            Prais = 23,
            [Display(Name = "VIDA TRES")]
            Vida_tres = 24,
            [Display(Name = "Familiar Médico")]
            Familiar_medico = 25,
            [Display(Name = "Ignorado")]
            Ignorado = 999,
            [Display(Name = "GES CONSALUD CATARATA")]
            Ges_Consalud_catarata = 1000,
            [Display(Name = "MÁS VIDA Doctor Luengo")]
            Mas_vida_luengo = 1001,
            [Display(Name = "Consulta GES Consalud")]
            Ges_Consalud_consulta = 1002,
            [Display(Name = "Consulta Más Vida Doctor Luengo")]
            Mas_vida_luengo_consulta = 1003,
            [Display(Name = "Consulta Cruz Blanca Doctor Luengo")]
            CruzBlca_luengo_consulta = 1004,
            [Display(Name = "Consulta Más Vida Doctor Águila")]
            MasVida_aguila_consulta = 1005,
            [Display(Name = "Consulta Cruz Blanca Doctor Águila")]
            CruzBlca_aguila_consulta = 1006,
            [Display(Name = "Consulta Más Vida Doctor Marquez")]
            MasVida_marquez_consulta = 1007,
            [Display(Name = "GES Más Vida")]
            Ges_MasVida = 1008,
            [Display(Name = "Consulta Bono Electrónico Banmédica")]
            Banmdica_econsulta = 1009,
            [Display(Name = "Exploración Bono Electrónico Banmédica")]
            Banmdica_eexploracion = 1010,
            [Display(Name = "Consulta Bono Electrónico Vida Tres")]
            Vida3_econsulta = 1011,
            [Display(Name = "Exploración Bono Electrónico Vida Tres")]
            Vida3_eexploracion = 1012,
            [Display(Name = "QUANTUM")]
            Quantum = 1013,
            [Display(Name = "Superintendencia de Pensiones")]
            Super_Pensiones = 1014,
            [Display(Name = "Particular Más Vida KB")]
            MasVida_Particular = 1015,
            [Display(Name = "Consulta Bono Web")]
            Econsulta = 1016,
            //[Display(Name = "Consulta Doctora Bogdanic Cruz Blanca")]
            //Banmdica_econsulta = 1017,
            //[Display(Name = "Consulta Doctora Marquez Cruz Blanca")]
            //Banmdica_econsulta = 1018,
            //[Display(Name = "Consulta Doctor Ramirez Cruz Blanca")]
            //Banmdica_econsulta = 1019,
            //[Display(Name = "GEA Chile")]
            //Banmdica_econsulta = 1020,
            //[Display(Name = "Nueva Más Vida")]
            //Banmdica_econsulta = 1021,
            //[Display(Name = "(No Usar) Nueva Más Vida")]
            //Banmdica_econsulta = 1022,
            //[Display(Name = "Consulta Doctor Claramunt Nueva Más Vida")]
            //Banmdica_econsulta = 1023,
            //[Display(Name = "Consulta Doctor Claramunt Consalud")]
            //Banmdica_econsulta = 1024,
            //[Display(Name = "Consulta Doctor Claramunt Colmena")]
            //Banmdica_econsulta = 1025,
            //[Display(Name = "Consulta Doctor Claramunt Cruz Blanca")]
            //Banmdica_econsulta = 1026,
            //[Display(Name = "Consulta Consalud")]
            //Banmdica_econsulta = 1027,
            //[Display(Name = "Exploración Consalud")]
            //Banmdica_econsulta = 1028,
            //[Display(Name = "Prueba")]
            //Banmdica_econsulta = 1029,
        }
    }