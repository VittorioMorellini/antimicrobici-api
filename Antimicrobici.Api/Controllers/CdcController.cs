using Antimicrobici.Api.Models;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AntimicrobiciApi.Controllers
{
    [Route("cdc")]
    [EnableCors]
    public class CdcController : ControllerBase
    {
        private readonly ICentriDiCostoService service;
        public CdcController(ICentriDiCostoService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Result<CentroDiCosto> Get()
        {
            Result<CentroDiCosto> result = null;
            List<CentroDiCosto> cdcs = new List<CentroDiCosto>();
            String userID = "siamorellini";

            cdcs = service.GetCentriDiCosto(userID);
            result = new Result<CentroDiCosto>(cdcs.Count, cdcs, false);

            return result;
        }

        // GET: api/Cdc/nome
        [HttpGet]
        [Route("nome")]
        public Result<CentroDiCosto> GetByNome(string nomeMateriale = "")
        {
            Result<CentroDiCosto> result = null;
            List<CentroDiCosto> cdcs = new List<CentroDiCosto>();
            String userID = "siamorellini";

            cdcs = service.SearchCentriDiCosto(nomeMateriale, userID);
            result = new Result<CentroDiCosto>(cdcs.Count, cdcs, false);

            return result;
        }

    }
}
