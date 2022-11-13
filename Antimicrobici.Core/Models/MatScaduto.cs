using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class MatScaduto
    {
        public long Id { get; set; }
        public string? InsertUser { get; set; }
        public DateTime InsertDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? CodCdc { get; set; }
        public DateTime? ControlDate { get; set; }
        public string? CodMateriale { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Lotto { get; set; }
        public string? Notes { get; set; }
        public string? CompileUser { get; set; }
        public string? QualificationUser { get; set; }
        public string? RetirementUser { get; set; }
        public DateTime? RetirementDate { get; set; }
        public string? RetirementNotes { get; set; }
    }
}
