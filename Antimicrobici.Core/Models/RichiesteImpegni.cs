using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class RichiesteImpegni
    {
        public long Id { get; set; }
        public string? CdcRichiedente { get; set; }
        public string? DescrizioneCdcRichiedente { get; set; }
        public string? CodiceMateriale { get; set; }
        public string? DescrizioneMateriale { get; set; }
        public string? NrImpegno { get; set; }
    }
}
