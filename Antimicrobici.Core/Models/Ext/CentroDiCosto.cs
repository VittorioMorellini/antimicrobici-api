using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
    [NotMapped]
    public class CentroDiCosto
    {
        public long? CompanyId { get; set; }
        public string Azienda { get; set; }
        public string CodCdc { get; set; }
        public string Cdc { get; set; }
        public string CodUO { get; set; }
        public string UO { get; set; }
    }
}
