using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class ConfirmPhoneSuccessModel : PageModel
    {
        public Uri ReturnUrl { get; set; }

        public void OnGet(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
    }
}