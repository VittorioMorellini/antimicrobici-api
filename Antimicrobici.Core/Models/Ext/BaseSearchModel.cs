using Antimicrobici.Core.Filters;
using Antimicrobici.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Antimicrobici.Core.Models
{
    public class BaseSearchModel : QueryBuilderSearchModel, IFilterModel
    {
        public long? PrincipalId { get; set; }
        public long? CompanyId { get; set; }
        public long? AgencyId { get; set; }
        public IEnumerable<long> ProductIds { get; set; }
        public IEnumerable<long> AgencyIds { get; set; }
        public string CustomerVisibility { get; set; }
    }
}
