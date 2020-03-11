using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IRegCivil
    {
        Task<bool> IsValidAsync(int rut, int carnet, bool isExt);
        //Task Init();
    }
}
