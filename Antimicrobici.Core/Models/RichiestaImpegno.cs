using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class RichiestaImpegno
    {
        public long Id { get; set; }
        public string? CdcRichiedente { get; set; }
        public string? DescrizioneCdcRichiedente { get; set; }
        public string? CodiceMateriale { get; set; }
        public string? DescrizioneMateriale { get; set; }
        public string? NrImpegno { get; set; }
        public int? Quantita { get; set; }
        public int? QtaRichiesta { get; set; }
        public string? Posologia { get; set; }
        public string? InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DataErogazione { get; set; }
        public string? Motivazione { get; set; }
        public string? MedicoRichiedente { get; set; }
        public string? DestinatarioPazienteDettaglio { get; set; }
        public DateTime? DataCreazioneImpegno { get; set; }
        public string? Note { get; set; }
        public string? ConsulenzaInfettivologica { get; set; }
        public string? MedicoUo { get; set; }
        public string? Antibiogramma { get; set; }
        public bool? Alert { get; set; }
        public string? TipoImpegno { get; set; }
    }
}
