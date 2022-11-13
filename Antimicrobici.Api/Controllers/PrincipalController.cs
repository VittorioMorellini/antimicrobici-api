using Antimicrobici.Core.Filters;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Antimicrobici.Api.Controllers
{
    //[Authorize]
    [Route("principal")]
    public class PrincipalController : ControllerBase
    {
        private readonly IPrincipalService service;
        public PrincipalController(IPrincipalService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("OK");
        }

        [HttpGet("{id}")]
        public IActionResult Find(long id)
        {
            return ServiceResult.Execute(() => service.Find(id));
        }

        [HttpPost("search")]
        [EnhanceModelFilter]
        public IActionResult Search([FromBody] PrincipalSearchModel model)
        {
            return ServiceResult.Execute(() => service.Search(model));
        }

        [HttpGet("exists/{username}")]
        public IActionResult PrincipalnameExists(string username)
        {
            return ServiceResult.Execute(() => service.PrincipalnameExists(username));
        }

        [HttpPost]
        public IActionResult Save([FromBody] Principal item)
        {
            return ServiceResult.Execute(() => service.Save(item.Id, item));
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] long id)
        {
            return ServiceResult.Execute(() => service.Delete(id));
        }
    }
}