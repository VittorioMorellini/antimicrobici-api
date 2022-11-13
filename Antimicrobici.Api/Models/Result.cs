using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Antimicrobici.Api.Models
{
  public class Result<T>
  {
    private Int32 _total;
    private List<T> _results;
    private bool _alert;

    public Result(Int32 total, List<T> results, bool alert)
    {
      _total = total;
      _results = results;
      _alert = alert;
    }

    public int total { get => _total; set => _total = value; }
    public List<T> results { get => _results; set => _results = value; }
    public bool alert { get => _alert; set => _alert = value; }
  }
}