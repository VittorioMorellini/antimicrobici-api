using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antimicrobici.Core.Filters
{
    public interface IFilterModel
    {
        public long? PrincipalId { get; set; }
        public long? CompanyId { get; set; }
        public IEnumerable<long> ProductIds { get; set; }
    }

    public class EnhanceModelFilterAttribute : TypeFilterAttribute
    {
        public EnhanceModelFilterAttribute() : base(typeof(EnhanceModelFilterAttributeImpl))
        {
        }

        private class EnhanceModelFilterAttributeImpl : IActionFilter
        {
            //private readonly IAppCache cache;
            //private readonly IIdentityService identityService;
            public EnhanceModelFilterAttributeImpl(/*IIdentityService identityService*/ )
            {
                //this.identityService = identityService;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.ActionArguments.TryGetValue("model", out object m))
                {
                    var model = m as IFilterModel;

                    //string key = $"identity_{identity.GetName()}";
                    //// HACK vmorell 20201026, recupero il Offset rispetto ad UTC da passare al calcolo della AbsoluteExpiration
                    //TimeSpan offset = DateTimeOffset.Now.Offset;
                    //var iy = cache.GetOrAdd(
                    //        key,
                    //        () => identity.GetIdentity(),
                    //        new MemoryCacheEntryOptions()
                    //        {
                    //            // Ora solare (1 ora da UTC)
                    //            AbsoluteExpiration = new DateTimeOffset(DateTime.Now, new TimeSpan(1, 0, 0)),
                    //            // AbsoluteExpiration = new DateTimeOffset(DateTime.Now, offset),
                    //            // Ora legale (2 ore da UTC)
                    //            //AbsoluteExpiration = new DateTimeOffset(DateTime.Now, new TimeSpan(2, 0, 0)),
                    //            SlidingExpiration = new TimeSpan(0, 5, 0)
                    //        }
                    //);
                    var identity = new Principal() { Role= "ADMIN" };//identityService.GetIdentity();
                    if (identity.Role != Role.ADMIN && !identity.CompanyId.HasValue)
                        throw new Exception("CompanyId is mandatory");

                    if (identity.Role != Role.ADMIN)
                        model.CompanyId = identity.CompanyId;

                    if (identity.Role != Role.ADMIN/* && identity.Role != Role.ADMIN*/) 
                    {
                        model.PrincipalId = identity.Id;
                    }
                    //model.ProductIds = identity.ProductIds;

                    context.ActionArguments["model"] = model;
                }
                else
                {
                    throw new Exception("Model parameter is mandatory");
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }
    }
}