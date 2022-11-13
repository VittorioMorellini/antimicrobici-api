using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
  public partial class Richiesta
  {
    public int? Quantita { get; set; }
    public string NrImpegno { get; set; }
    public string TipoImpegno { get; set; }
    public int Stato { get; set; }
    public NamedEntity CdcRichiedente { get; set; }
    public DateTime? InserimentoData { get; set; }
    public string InserimentoUtente { get; set; }
    public DateTime? ModificaData { get; set; }
    public string ModificaUtente { get; set; }
    public int QtaRichiesta { get; set; }
    public string CodiceMateriale { get; set; }
    public string DescrizioneMateriale { get; set; }
    public DateTime? DataErogazione { get; set; }
    public string Motivazione { get; set; }
    public string MedicoRichiedente { get; set; }
    public string DestinatarioPazienteDettaglio { get; set; }
    public DateTime? DataCreazioneImpegno { get; set; }
    public string Note { get; set; }
    public string ConsulenzaInfettivologica { get; set; }
    public string MedicoUO { get; set; }
    public string Antibiogramma { get; set; }
    public int StatoMedico { get; set; }
    public string NoteMedico { get; set; }
    public DateTime? ModificaDataMedico { get; set; }
    public string ModificaUtenteMedico { get; set; }
    public string Posologia { get; set; }
    public Boolean OffLabel { get; set; }
    public Boolean Alert { get; set; }

  }
}
