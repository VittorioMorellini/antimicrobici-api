using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Core.Models
{
    public class MenuEntry
    {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public MenuEntry[] items { get; set; }
    }

    public class MenuProfile
    {
        public string landingPage { get; set; }
        public MenuEntry[] menu { get; set; }
        public bool isInfettivologo { get; set; }
    }


}