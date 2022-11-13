using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace AntimicrobiciApi.Controllers
{
    [Route("stampe")]
    [EnableCors]
    public class StampeController : ControllerBase
    {

        private readonly IMenuService service;
        private readonly ILogger<StampeController> logger;
        public StampeController(IMenuService service, ILogger<StampeController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpGet]
        // GET: api/Stampe/5
        public string Get(string urlMenu = "")
        {
            #region DECLARATION
            String urlReport = String.Empty;
            string userID = String.Empty;
            #endregion

            if (String.IsNullOrWhiteSpace(urlMenu))
                return String.Empty;

            try
            {
                // Authentication
                userID = "siamorellini";
                string[] pathUrl = urlMenu.Split('/');
                // Log.logInfo("Sto per eseguire Query con EF");
                Menu menu = service.GetSingoloMenu(pathUrl != null && pathUrl.Length >= 1 ? pathUrl[1] : "");
                if (menu != null)
                    urlReport = menu.UrlReport;
            }
            catch (Exception ex)
            {
                logger.LogError("Errore generico EF", ex);
                throw;
            }

            return urlReport;
        }

    }
}
