using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class ConfirmPhoneSuccessModel : PageModel
    {
        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
    }
}