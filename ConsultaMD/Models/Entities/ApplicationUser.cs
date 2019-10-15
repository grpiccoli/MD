using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ConsultaMD.Models.Entities
{
    // Add profile data for application users by adding properties to the AppUser class
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime PhoneConfirmationTime { get; set; }
        public DateTime MailConfirmationTime { get; set; }
        public bool IsActive { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; } = new List<ApplicationUserRole>();
        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();
    }
}

