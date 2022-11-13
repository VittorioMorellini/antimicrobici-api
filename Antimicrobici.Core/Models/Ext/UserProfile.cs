using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Core.Models
{
    public class UserProfile
    {
        public string id { get; set; }
        public string domain { get; set; }
        public string landingPage { get; set; }
        public string descrizione { get; set; }
        public bool isInfettivologo { get; set; }

    }
}