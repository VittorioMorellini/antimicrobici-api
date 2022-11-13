using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class Principal
    {
        public Principal()
        {
            PrincipalGroup = new HashSet<PrincipalGroup>();
        }

        public long Id { get; set; }
        public string? Password { get; set; }
        public string? LandingPage { get; set; }
        public string? Description { get; set; }
        public string? Qualification { get; set; }
        public DateTime? ModifyPasswordDate { get; set; }
        public DateTime? WrongAccessDate { get; set; }
        public DateTime? LockUserDate { get; set; }
        public int? NumberWrongAccess { get; set; }
        public bool? Locked { get; set; }
        public bool? Disabled { get; set; }
        public bool? PasswordLocked { get; set; }
        public bool? ModifyPassword { get; set; }
        public string? Role { get; set; }
        public long? CompanyId { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<PrincipalGroup> PrincipalGroup { get; set; }
    }
}
