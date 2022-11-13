using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class Menu
    {
        public Menu()
        {
            GroupMenu = new HashSet<GroupMenu>();
            InverseParent = new HashSet<Menu>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
        public long? ParentId { get; set; }
        public int? Ordering { get; set; }
        public string? UrlReport { get; set; }
        public string? Description { get; set; }

        public virtual Menu? Parent { get; set; }
        public virtual ICollection<GroupMenu> GroupMenu { get; set; }
        public virtual ICollection<Menu> InverseParent { get; set; }
    }
}
