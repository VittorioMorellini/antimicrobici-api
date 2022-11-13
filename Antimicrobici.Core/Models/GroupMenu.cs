using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class GroupMenu
    {
        public long Id { get; set; }
        public long MenuId { get; set; }
        public long GroupId { get; set; }
        public bool ToExec { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}
