using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class MatScadutoCatalogo
    {
        public long Id { get; set; }
        public string Azienda { get; set; } = null!;
        public string? CodMateriale { get; set; }
        public string? DescMateriale { get; set; }
        public string? CodUnitaMisura { get; set; }
        public string? DescUnitaMisura { get; set; }
        public string? CodTipoMateriale { get; set; }
        public string? DescTipoMateriale { get; set; }
        public string? CodGruppoMerceologico { get; set; }
        public long? CompanyId { get; set; }

        public virtual Company? Company { get; set; }
    }
}
