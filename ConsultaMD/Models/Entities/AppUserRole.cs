using Microsoft.AspNetCore.Identity;

namespace ConsultaMD.Models
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public string RoleAssigner { get; set; }
    }
}
