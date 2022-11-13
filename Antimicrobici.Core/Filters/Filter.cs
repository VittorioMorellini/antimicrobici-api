using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Core.Filters
{
    public class Filter
    {
    }

    public class PaginatedFilter
    {
        public Int32 Offset { get; set; }
        public Int32 Count { get; set; }
    }

    public class RichiestaFilter : PaginatedFilter
    {
        public string CodiceRichiedente { get; set; }
        public string CodiceMateriale { get; set; }
        public string Medico { get; set; }
        public string Motivazione { get; set; }
        public string Destinatario { get; set; }
        public string Stato { get; set; }
        public string StatoMedico { get; set; }
        public DateTime? DataDa { get; set; }
        public DateTime? DataA { get; set; }
        public string Errore { get; set; }

        public string sortBy { get; set; }
        public Int32? sortDir { get; set; }
    }


    public class MatScadutoFilter : PaginatedFilter
    {
        public string UnitaOperativa { get; set; }
        public string CodiceRichiedente { get; set; }
        public string Materiale { get; set; }
        public string TipoMateriale { get; set; }
        public string Cdc { get; set; }
        public string Stato { get; set; }
        public DateTime? DataDa { get; set; }
        public DateTime? DataA { get; set; }
        public string Errore { get; set; }

        public string sortBy { get; set; }
        public Int32? sortDir { get; set; }
    }

}