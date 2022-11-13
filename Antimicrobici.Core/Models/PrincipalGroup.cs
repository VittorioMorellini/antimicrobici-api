using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class PrincipalGroup
    {
        public long Id { get; set; }
        public long PrincipalId { get; set; }
        public long GroupId { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual Principal Principal { get; set; } = null!;
    }
}
