using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IMIP
    {
        Task<bool> Validate(int insurance, int rut, string pwd);
    }
}
