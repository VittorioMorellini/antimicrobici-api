using Antimicrobici.Api.Models;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Antimicrobici.Api.Controllers
{
    [Route("catalogo")]
    [EnableCors]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService service;
        public CatalogoController(ICatalogoService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Result<MatScadutoCatalogo> Get()
        {
            string userID = "siamorellini";
            List<MatScadutoCatalogo> results = service.GetMaterialiScadutiCatalogo();

            return new Result<MatScadutoCatalogo>(results.Count, results, false);
        }

        [HttpGet]
        [Route("nome")]
        public Result<MatScadutoCatalogo> GetMaterialiScadutiCatalogoByNome(string nomeMateriale = "")
        {
            string userID = "siamorellini";
            List<MatScadutoCatalogo> results = service.GetMaterialiScadutiCatalogoByNome(nomeMateriale);

            return new Result<MatScadutoCatalogo>(results.Count, results, false);
        }

    }
}
