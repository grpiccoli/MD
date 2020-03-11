using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface ISuperSaludService
    {
        Task<SuperData> GetDr(int rut);
    }
}
