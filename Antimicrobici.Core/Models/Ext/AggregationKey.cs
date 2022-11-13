using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Core.Models
{
  public class AggregationKey
  {
    public string key { get; set; }
    public NamedEntity value { get; set; }
    public Int32 count { get; set; }
    public AggregationKey[] keys { get; set; }
  }
}