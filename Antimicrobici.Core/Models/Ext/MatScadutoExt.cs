using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
    public partial class MatScaduto
    {
        [NotMapped]
        public string TooltipNotes { get; set; }
        [NotMapped]
        public CentroDiCosto Cdc { get; set; }
        [NotMapped]
        public NamedEntity Materiale { get; set; }
        [NotMapped]
        public NamedEntity Type { get; set; }
    }
}
