using ConsultaMD.Areas.Identity.Pages.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IMedium
    {
        Task Add(DoctorLocationsInputModel m);
        Task Delete(int id);
        Task Enable(int id);
        Task Disable(int id);
    }
}
