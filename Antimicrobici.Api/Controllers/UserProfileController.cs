using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
//using System.Web.Http.Cors;
//using AntimicrobiciApi.Helpers;
//using AntimicrobiciApi.Models;
//using AntimicrobiciDAL.Classes;
//using AntimicrobiciUtil.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Antimicrobici.Api.Controllers
{
    // [Authorize]
    [Route("user")]
    [EnableCors()]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService service;
        public UserProfileController(IUserProfileService service)
        {
            this.service = service;
        }

        // GET: api/user
        [HttpGet]
        public IActionResult Get()
        {
            return ServiceResult.Execute(() => service.GetProfile());
        }

    }
}
