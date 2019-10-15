using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ConsultaMD.Models.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IPAddress { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; } = new List<ApplicationUserRole>();
    }
}