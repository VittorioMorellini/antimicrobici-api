using Antimicrobici.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Api.Models
{
    public partial class MatScadutoRecord
    {
        public long? Id { get; set; }
        public DateTime? ControlDate { get; set; }
        public CentroDiCosto? Cdc { get; set; }
        public string CodCdc { get; set; }
        public NamedEntity? Materiale { get; set; }
        public string? CodMateriale { get; set; }
        public string? DescMateriale { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Notes { get; set; }
        public int Quantity { get; set; }
        public string? Lotto { get; set; }
    }

}
