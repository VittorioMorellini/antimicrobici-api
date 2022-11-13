using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class MatScadutoStruttura
    {
        public long Id { get; set; }
        public string Azienda { get; set; } = null!;
        public string CodCdc { get; set; } = null!;
        public string? Cdc { get; set; }
        public string? CodUo { get; set; }
        public string? Uo { get; set; }
        public string? CodDipartimento { get; set; }
        public string? Dipartimento { get; set; }
        public string? CodCdcOriginale { get; set; }
        public string? CdcOriginale { get; set; }
        public long? CompanyId { get; set; }

        public virtual Company? Company { get; set; }
    }
}
