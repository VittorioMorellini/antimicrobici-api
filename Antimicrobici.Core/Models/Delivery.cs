using System;
using System.Collections.Generic;

namespace Antimicrobici.Core.Models
{
    public partial class Delivery
    {
        public long Id { get; set; }
        public string? NrImpegno { get; set; }
        public int? Quantity { get; set; }
        public byte? Status { get; set; }
        public string? InsertUser { get; set; }
        public DateTime InsertDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Notes { get; set; }
        public byte? MedicalStatus { get; set; }
        public string? MedicalNotes { get; set; }
        public string? MedicalInsertUser { get; set; }
        public DateTime MedicalInsertDate { get; set; }
        public string? MedicalUpdateUser { get; set; }
        public DateTime MedicalUpdateDate { get; set; }
        public byte? OffLabel { get; set; }
    }
}
