using ConsultaMD.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IFonasa
    {
        Task Init();
        Task CloseBW();
        Task<Fonasa> GetById(int id);
        Task<FonasaWebPay> Pay(PaymentData paymentData);
    }
}
