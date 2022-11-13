using Antimicrobici.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Api.Models
{
  public class ResultExt<T>
  {
    private Int32 _total;
    private List<T> _results;
    private Dictionary<String, AggregationKey[]> _aggregations;

    public ResultExt(Int32 total, List<T> results, Dictionary<String, AggregationKey[]> aggregations)
    {
      _total = total;
      _results = results;
      _aggregations = aggregations;
    }

    public int total { get => _total; set => _total = value; }
    public List<T> results { get => _results; set => _results = value; }
    public Dictionary<String, AggregationKey[]> aggregations { get => _aggregations; set => _aggregations = value; }
  }
}
