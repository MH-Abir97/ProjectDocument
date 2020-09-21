using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Pronali.Data.Enum;
using Pronali.Data.Models.Interfaces;

namespace Pronali.Data.Models
{
    public class ApplicationUser : IdentityUser, IBaseModel
    {
        public UserType UserType { get; set; }
        public string Mod { get; set; }
        public string PublicIp { get; set; }
        public string PrivateIp { get; set; }
        public string MacAddress { get; set; }
        public string Vpn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string AvartarUrl { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

    }
}
