using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultaMD.Areas.MDs.Models
{
    public class SummaryVM
    {
        [Display(Name = "Citas Agendadas")]
        public int TotalAppointments { get; set; }
        [Display(Name = "Citas Hoy")]
        public int TodayAppointments { get; set; }
        [Display(Name = "Total Ingresos")]
        [DataType(DataType.Currency)]
        public int TotalIncome { get; set; }
        [Display(Name = "Total Facturado")]
        [DataType(DataType.Currency)]
        public int TotalInvoice { get; set; }
        [Display(Name = "Facturas Pendientes")]
        public int PendingInvoice { get; set; }
        [Display(Name = "Total Depositado")]
        [DataType(DataType.Currency)]
        public int TotalPayed { get; set; }
        [Display(Name = "Pagos Pendientes")]
        public int PendingPayments { get; set; }
        [Display(Name = "Total Cupos")]
        public int TimeSlots { get; set; }
        [Display(Name = "% Ocupación de Agenda")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public int Ocupacion 
        {
            get
            {
                return TimeSlots == 0 ? 0 : TakenTimeSlots/TimeSlots;
            }
            private set
            {
                Ocupacion = value;
            }
        }
        [Display(Name = "Total Citas Agendadas")]
        public int TakenTimeSlots { get; set; }
        [Display(Name = "Ingresos por Previsión")]
        public List<int> RevenuePerInsurance { get; } = new List<int>();
        [Display(Name = "Pacientes Totales")]
        public int TotalPatients { get; set; }
        [Display(Name = "Pacientes Antiguo")]
        public int ReturningPatients { get; set; }
        [Display(Name = "Comisión Total")]
        public int Comisiones { get; set; }
    }
}
