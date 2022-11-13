using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
    [NotMapped]
    public partial class NamedEntity
    {
        public NamedEntity()
        {
        }

        public NamedEntity(string codice, string nome)
        {
            this.Codice = codice;
            this.Nome = nome;
        }

        public NamedEntity(string codice)
        {
            this.Codice = codice;
        }

        public string Codice { get; set; }
        public string Nome { get; set; }
    }
}
