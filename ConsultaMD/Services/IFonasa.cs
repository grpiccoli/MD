using ConsultaMD.Models.VM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IFonasa
    {
        Task Init();
        Task CloseBW();
        Task<Fonasa> GetById(int id);
        Task<FonasaWebPay> Pay(PaymentData paymentData);
        Task<List<DocFonasa>> GetDocData(int id);
    }
}
