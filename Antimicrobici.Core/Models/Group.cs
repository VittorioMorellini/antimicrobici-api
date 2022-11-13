using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class Group
    {
        public Group()
        {
            GroupMenu = new HashSet<GroupMenu>();
            PrincipalGroup = new HashSet<PrincipalGroup>();
        }

        public long Id { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<GroupMenu> GroupMenu { get; set; }
        public virtual ICollection<PrincipalGroup> PrincipalGroup { get; set; }
    }
}
