using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Antimicrobici.Core.Services;
//using AntimicrobiciApi.Helpers;
//using AntimicrobiciApi.Models;
//using AntimicrobiciDAL;
//using AntimicrobiciDAL.Classes;
// using AntimicrobiciDAL.Classes;
using Antimicrobici.Core.Utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Antimicrobici.Api.Controllers
{
    // [Authorize(Users ="MAPS1\vimo")]
    // [Authorize]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [EnableCors()]
    [Route("menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService service;
        public MenuController(IMenuService service)
        {
            this.service = service;
        }

        // GET: menu
        [HttpGet]
        public IActionResult Get()
        {
            return ServiceResult.Execute(() => service.GetMenuProfile());

        }

    }
}
