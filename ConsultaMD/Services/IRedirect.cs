using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IRedirect
    {
        Task<string> Init(string userName);
        Task<IActionResult> Redirect(Uri returnUrl, string rut);
        string GetPage();
    }
}
