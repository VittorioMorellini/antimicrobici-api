using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;

namespace Antimicrobici.Api.Controllers
{
    //[Authorize]
    [Route("auth/identity")]
    public class IdentityController : ControllerBase
    {
        private IIdentityService service;
        public IdentityController(IIdentityService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("{username}/{password}")]
        public IActionResult c(string username, string password)
        {
            return ServiceResult.Execute(() => service.Authenticate(username, password));
        }

        //[HttpGet]
        //[Route("authenticate/{username}/{password}")]
        //public IActionResult Authenticate(string username, string password)
        //{
        //    var response = service.Authenticate(username, password);

        //    if (response == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(response);
        //}
        //[HttpGet("filter")]
        //public IActionResult Filter()
        //{
        //    return ServiceResult.Execute(() => service.GetFilter());
        //}     
    }
}
