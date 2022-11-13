using Antimicrobici.Api.Models;
using Antimicrobici.Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Antimicrobici.Api.Controllers
{
    [Route("tipimateriale")]
    [EnableCors]
    public class TipiMaterialeController : ControllerBase
    {
        private readonly ITipoMaterialeService service;
        public TipiMaterialeController(ITipoMaterialeService service)
        {
            this.service = service;
        }

        [HttpGet]
        // GET: api/TipiMateriale
        public Result<NamedEntity> Get()
        {
            Result<NamedEntity> result = null;
            List<NamedEntity> tipis = new List<NamedEntity>();

            String userID = "siamorellini";

            tipis = service.GetTipiMateriale(userID);
            result = new Result<NamedEntity>(tipis.Count, tipis, false);

            return result;
        }

    }
}
