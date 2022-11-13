using Antimicrobici.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Antimicrobici.Api.Helpers;
using Antimicrobici.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Antimicrobici.Core.Services;

namespace Antimicrobici.Api.Controllers
{
    [EnableCors()]
    [Route("richiedente")]
    public class RichiedenteController : ControllerBase
    {
        private readonly IRichiedenteService service;
        private readonly ILogger<IRichiedenteService> logger;
        public RichiedenteController(IRichiedenteService service, ILogger<IRichiedenteService> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        // GET: api/Reparti
        [HttpGet]
        public Result<Richiedente> Get()
        {
            Result<Richiedente> result = null;
            List<Richiedente> repartos = new List<Richiedente>();

            String username = "siamorellini";

            IEnumerable<Richiedente> repartis = service.GetRichiedenti(username);
            foreach (Richiedente rep in repartis)
            {
                Richiedente item = new Richiedente();
                item.Codice = rep.Codice;
                item.Nome = rep.Nome;
                repartos.Add(item);
            }
            result = new Result<Richiedente>(repartos.Count, repartos, false);
            return result;
        }

    }
}
