using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class Company
    {
        public Company()
        {
            MatScadutoCatalogo = new HashSet<MatScadutoCatalogo>();
            MatScadutoStruttura = new HashSet<MatScadutoStruttura>();
            Principal = new HashSet<Principal>();
        }

        public long Id { get; set; }
        public string BusinessName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string VatCode { get; set; } = null!;
        public DateTime InsertDate { get; set; }
        public string InsertUser { get; set; } = null!;
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; } = null!;
        public string? Mail { get; set; }
        public string? TaxCode { get; set; }
        public string? Code { get; set; }

        public virtual ICollection<MatScadutoCatalogo> MatScadutoCatalogo { get; set; }
        public virtual ICollection<MatScadutoStruttura> MatScadutoStruttura { get; set; }
        public virtual ICollection<Principal> Principal { get; set; }
    }
}
