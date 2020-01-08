using System;

namespace ConsultaMD.Models.Entities
{
    //Flow Customer Info
    public class Customer
    {
        public string CustomerId { get; set; }
        public DateTime Created { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public PayMode PayMode { get; set; }
        public string CreditCardType { get; set; }
        public string Last4CardDigits { get; set; }
        public int ExternalId { get; set; }
        public virtual Person External { get; set; }
        public Status Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Token { get; set; }
    }
    public enum PayMode
    {
        auto, 
        manual
    }
    public enum Status
    {
        Inactivo = 0,
        Activo = 1,
        Eliminado = 3
    }
}
