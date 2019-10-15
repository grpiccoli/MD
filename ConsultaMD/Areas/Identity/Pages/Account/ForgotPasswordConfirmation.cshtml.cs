using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        public string Email { get; set; }
        public void OnGet(string maskedEmail = null)
        {
            Email = maskedEmail;
        }
    }
}
