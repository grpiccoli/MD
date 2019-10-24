using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Areas.Patients.Views.Search;
using ConsultaMD.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaMD.Areas.Patients.Controllers
{
    [Area("Patients")]
    public class TmpController : Controller
    {
        public IActionResult DoctorSummaryData()
        {
            var data = new
            {
                Rut_beneficiario = "12.345.678-0",
                Nombre = "Elaine R. Winters",
                Rut_titular = "12.345.678-0",
                Rut_paciente = "12.345.678-0",
                Doctor = "Elaine R. Winters",
                Prevision = "Consalud",
                Total = "$9,600",
                Valor_bono = "$9,600",
                Copago = "$9,600",
            };

            return Json(data);
        }

        public IActionResult AddedDependantsData()
        {
            var puntos = new[] {
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" },
                new { Rut = "12.345.678-0", Nombre = "Elaine R. Winters" }
            };

            return Json(puntos);
        }

        public JsonResult BuscarList()
        {
            var tl = new List<object> {
                new
                {
                    label = "Oftalmología",
                    id = 1,
                    choices = new List<object>
                    {
                        new {
                            value = "9649928",
                            label = "Hugo Menares Rodríguez"
                        },
                        new {
                            value = "12752234",
                            label = "Marco Luengo López"
                        },
                        new {
                            value = "12116504",
                            label = "Jaime Claramunt Llull"
                        },
                        new {
                            value = "9643638",
                            label = "Mauricio Paris González"
                        },
                        new {
                            value = "16055808",
                            label = "Sebastian Andres Ramirez"
                        },
                        new {
                            value = "15625915",
                            label = "Ignacio Alfaro Quezada"
                        },
                        new {
                            value = "8749179",
                            label = "Leonardo Fernández Hinrichs"
                        },
                        new {
                            value = "15734669",
                            label = "Carlos Bachmann Galle"
                        },
                        new {
                            value = "13657332",
                            label = "Juan Pablo Marquez Doren"
                        },
                        new {
                            value = "11736802",
                            label = "Cristian Águila Rebolledo"
                        },
                        new {
                            value = "6264576",
                            label = "Alejandro Siebert Eller"
                        },
                        new {
                            value = "12670698",
                            label = "Alejandro Morales Mac-Hale"
                        },
                        new {
                            value = "12498240",
                            label = "Cristian Montecinos Contreras"
                        }
                    }
                }
            };
            return Json(tl);
        }

        public IActionResult MapList1()
        {
            var drs = new MapDocVM[] {
                new MapDocVM{
                    run = 9649928,
                    dr = "Hugo Menares Rodríguez",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName(),
                    experience = 28
                },
                new MapDocVM{
                    run = 12752234,
                    dr = "Marco Luengo López",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName(),
                    experience = 20
                },
                new MapDocVM{
                    run = 12116504,
                    dr = "Jaime Claramunt Llull",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName(),
                    experience = 19
                },
                new MapDocVM{
                    run = 9643638,
                    dr = "Mauricio Paris González",
                    experience = 28,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 16055808,
                    dr = "Sebastian Andres Ramirez",
                    experience = 12,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 15625915,
                    dr = "Ignacio Alfaro Quezada",
                    experience = 10,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 8749179,
                    dr = "Leonardo Fernández Hinrichs",
                    experience = 25,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 15734669,
                    dr = "Carlos Bachmann Galle",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 13657332,
                    dr = "Juan Pablo Marquez Doren",
                    experience = 13,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 11736802,
                    dr = "Cristian Águila Rebolledo",
                    experience = 22,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 6264576,
                    dr = "Alejandro Siebert Eller",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 12670698,
                    dr = "Alejandro Morales Mac-Hale",
                    experience = 25,
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
                new MapDocVM{
                    run = 12498240,
                    dr = "Cristian Montecinos Contreras",
                    especialidad = Data.EspecialidadesData.Especialidad.Oftalmologia.GetAttrName()
                },
            };
            var places = new MapPlaceVM[] {
                new MapPlaceVM
                {
                    placeId = "ChIJAQAH8QQ7GJYRfSMm64pWO6I", //PTO MONTT
                    items = new MapItemVM[]
                    {
                        new MapItemVM{
                            run = 9649928,
                            price = 5505
                        },
                        new MapItemVM{
                            run = 12752234,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 12116504,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 9643638,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 16055808,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 15625915,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 8749179,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 15734669,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 13657332,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 11736802,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 12670698,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 12498240,
                            price = 5505,
                        },
                    }
                },
                new MapPlaceVM
                {
                    placeId = "ChIJ93HnSaUnGJYRBQ0t-jO6cD0", //PTO VARAS
                    items = new MapItemVM[]
                    {
                        new MapItemVM{
                            run = 9649928,
                            price = 5505
                        },
                        new MapItemVM{
                            run = 12116504,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 9643638,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 15625915,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 11736802,
                            price = 5505,
                        },
                        new MapItemVM{
                            run = 6264576,
                            price = 5505,
                        },
                    }
                },
                new MapPlaceVM
                {
                    placeId = "ChIJ78uE8Kw7GJYRQVVy-ksmv8c", //BAQUEDANO
                    items = new MapItemVM[]
                    {
                        new MapItemVM{
                            run = 9649928,
                            price = 5505
                        },
                        new MapItemVM{
                            run = 9643638,
                            price = 5505,
                        },
                    }
                }
            };

            return Json(new { drs, places });
        }
        public IActionResult MapList2()
        {
            var puntos = new[] {
    new { Id=1, Nombre = "Hop Meyers", Texto = "Lorem ipsum dolor sit amet, ", Latitud = "-41.2016800586", Longitud = "-73.0153836742", Imagen = 8, Costo = "6, 246", Clinica = "Libero Foundation", Especialidad = "Legal Department", Experiencia = 4, Valoracion = 4 },
new { Id=1, Nombre = "Aaron Y. Copeland", Texto = "Lorem ipsum dolor sit amet, consectetuer", Latitud = "-41.6184447682", Longitud = "-73.6096959745", Imagen = 3, Costo = "5, 505", Clinica = "Donec Nibh Enim Corporation", Especialidad = "Media Relations", Experiencia = 2, Valoracion = 1 },
new { Id=1, Nombre = "Flavia Sampson", Texto = "Lorem ipsum dolor sit amet, consectetuer", Latitud = "-41.5936811718", Longitud = "-73.5689352074", Imagen = 4, Costo = "8, 405", Clinica = "Ac Mattis Semper Foundation", Especialidad = "Research and Development", Experiencia = 3, Valoracion = 5 },
new { Id=1, Nombre = "Caldwell Reed", Texto = "Lorem ipsum dolor", Latitud = "-41.2616462011", Longitud = "-73.0043793442", Imagen = 6, Costo = "9, 585", Clinica = "Nibh Sit Amet Limited", Especialidad = "Customer Relations", Experiencia = 7, Valoracion = 3 },
new { Id=1, Nombre = "Dillon Butler", Texto = "Lorem ipsum dolor sit amet, ", Latitud = "-41.6441791752", Longitud = "-73.6808584251", Imagen = 8, Costo = "8, 475", Clinica = "Nunc Quis Arcu Limited", Especialidad = "Quality Assurance", Experiencia = 9, Valoracion = 1 },
new { Id=1, Nombre = "Hu V. Gray", Texto = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Curabitur sed", Latitud = "-41.3139266556", Longitud = "-72.9676130371", Imagen = 3, Costo = "9, 267", Clinica = "Sodales Nisi Magna Consulting", Especialidad = "Accounting", Experiencia = 1, Valoracion = 1 },
new { Id=1, Nombre = "Richard Anthony", Texto = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.", Latitud = "-41.6014156406", Longitud = "-73.5839984933", Imagen = 7, Costo = "9, 907", Clinica = "Nec Limited", Especialidad = "Payroll", Experiencia = 2, Valoracion = 4 },
new { Id=1, Nombre = "Aurelia Randolph", Texto = "Lorem ipsum", Latitud = "-41.5255071983", Longitud = "-73.4741995850", Imagen = 3, Costo = "6, 534", Clinica = "Mauris Rhoncus Id Industries", Especialidad = "Accounting", Experiencia = 10, Valoracion = 4 },
new { Id=1, Nombre = "Giacomo Lamb", Texto = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Curabitur", Latitud = "-41.2674182567", Longitud = "-73.0104435081", Imagen = 2, Costo = "8, 034", Clinica = "Mauris Vel Turpis Incorporated", Especialidad = "Payroll", Experiencia = 5, Valoracion = 2 },
new { Id=1, Nombre = "Rhonda J. Montoya", Texto = "Lorem ipsum dolor sit amet, consectetuer adipiscing", Latitud = "-41.2499326977", Longitud = "-73.0106688137", Imagen = 2, Costo = "5, 555", Clinica = "Mus Proin Ltd", Especialidad = "Human Resources", Experiencia = 2, Valoracion = 2 },
new { Id=1, Nombre = "Judah U. Wagner", Texto = "Lorem ipsum dolor", Latitud = "-41.3960852167", Longitud = "-73.4627697450", Imagen = 4, Costo = "6, 421", Clinica = "Lorem Industries", Especialidad = "Payroll", Experiencia = 4, Valoracion = 3 },
new { Id=1, Nombre = "Jasper L. Solomon", Texto = "Lorem ipsum dolor", Latitud = "-41.3287732677", Longitud = "-72.9643527557", Imagen = 7, Costo = "6, 065", Clinica = "Imperdiet Erat Incorporated", Especialidad = "Payroll", Experiencia = 2, Valoracion = 3 },
new { Id=1, Nombre = "Teegan Britt", Texto = "Lorem ipsum", Latitud = "-41.3900473428", Longitud = "-72.9098502685", Imagen = 1, Costo = "8, 938", Clinica = "Justo Proin Non PC", Especialidad = "Public Relations", Experiencia = 10, Valoracion = 3 },
new { Id=1, Nombre = "Ignacia Calderon", Texto = "Lorem ipsum dolor sit", Latitud = "-41.4416695413", Longitud = "-72.9239265014", Imagen = 5, Costo = "8, 738", Clinica = "Eu Accumsan Corporation", Especialidad = "Asset Management", Experiencia = 6, Valoracion = 5 },
new { Id=1, Nombre = "Lee Summers", Texto = "Lorem ipsum dolor sit", Latitud = "-41.3961247042", Longitud = "-73.4609220825", Imagen = 3, Costo = "8, 219", Clinica = "Fringilla Mi Incorporated", Especialidad = "Finances", Experiencia = 1, Valoracion = 3 },
new { Id=1, Nombre = "Elmo A. Battle", Texto = "Lorem ipsum dolor sit amet, ", Latitud = "-41.3962612635", Longitud = "-73.4609452435", Imagen = 7, Costo = "7, 441", Clinica = "Sapien Associates", Especialidad = "Sales and Marketing", Experiencia = 10, Valoracion = 4 },
new { Id=1, Nombre = "Susan Brown", Texto = "Lorem ipsum dolor sit amet, consectetuer", Latitud = "-41.4348554418", Longitud = "-73.0438190510", Imagen = 8, Costo = "7, 283", Clinica = "Velit Egestas Lacinia Associates", Especialidad = "Accounting", Experiencia = 4, Valoracion = 3 },
new { Id=1, Nombre = "Rachel Mendez", Texto = "Lorem ipsum", Latitud = "-41.6555539162", Longitud = "-73.5979796374", Imagen = 3, Costo = "6, 698", Clinica = "A Odio Corp.", Especialidad = "Public Relations", Experiencia = 6, Valoracion = 2 },
new { Id=1, Nombre = "Tyrone F. Riggs", Texto = "Lorem ipsum dolor sit amet, consectetuer adipiscing", Latitud = "-41.6100385038", Longitud = "-73.6509157145", Imagen = 4, Costo = "6, 529", Clinica = "Enim Consequat Purus Consulting", Especialidad = "Asset Management", Experiencia = 9, Valoracion = 2 }
};
            return Json(puntos);
        }
        public IActionResult BookingMorning()
        {
            var morning = new[]
            {
                new {hora="9:00"},new {hora="10:00"},new {hora="11:00"}
            };
            return Json(morning);

        }

        public IActionResult BookingAfternoon()
        {
            var afternoon = new[]
            {
                new {hora="13:00"},new {hora="16:00"},new {hora="17:00"}
            };
            return Json(afternoon);
        }

        public IActionResult TransactionHistoryList()
        {
            var list = new[]
            {
                new {Id=12341, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12342, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Pendiente"},
                new {Id=12343, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12344, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12345, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Pendiente"},
                new {Id=12346, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12347, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12348, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12349, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12350, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"},
                new {Id=12351, Doctor="Dr Alexa William", Experiencia="6 Años",Pago="Efectivo",Abono="$2.500",Reembolso="$500",Transaccion="TI235678",Estado="Aprobado"}
            };
            return Json(list);
        }
    }
}