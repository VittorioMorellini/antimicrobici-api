using Antimicrobici.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Antimicrobici.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;

namespace AntimicrobiciApi.Controllers
{
    [Route("materiali")]
    [EnableCors()]
    public class MaterialiController : ControllerBase
    {
        private readonly IMaterialiService materialiService;
        public MaterialiController(IMaterialiService materialiService) 
        {
            this.materialiService = materialiService;
        }

        [HttpGet]
        public Result<NamedEntity> Get()
        {
            Result<NamedEntity> result = null;
            List<NamedEntity> results = new List<NamedEntity>();
            String username = "siamorellini";

            List<NamedEntity> materialis = materialiService.GetMateriali(username);
            foreach (NamedEntity rep in materialis)
            {
                NamedEntity item = new NamedEntity();
                item.Codice = rep.Codice;
                item.Nome = rep.Nome;
                results.Add(item);
            }
            result = new Result<NamedEntity>(results.Count, results, false);

            return result;
        }
    }
}
