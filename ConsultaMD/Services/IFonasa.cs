using ConsultaMD.Models.VM;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IFonasa
    {
        //Task Init();
        //Task CloseBW();
        Task<Fonasa> GetByIdAsync(int id, bool doc = false);
        Task<WebPayResponse> PayAsync(PaymentData paymentData);
        //Task<List<DocFonasa>> GetDocDataAsync(int id);
    }
}
