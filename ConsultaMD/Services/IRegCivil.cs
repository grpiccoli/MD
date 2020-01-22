using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IRegCivil
    {
        //Task<bool> Init();
        //Task CloseBW();
        Task<bool> IsValid(int rut, int carnet, bool isForeign);
    }
}
