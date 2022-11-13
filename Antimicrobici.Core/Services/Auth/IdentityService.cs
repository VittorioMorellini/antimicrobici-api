using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
// using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Antimicrobici.Core.Models;
//using Antimicrobici.Core.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Antimicrobici.Core.Models.Auth;

namespace Antimicrobici.Core.Services
{
    public interface IIdentityService
    {

        ClaimsPrincipal GetUser();
        string GetSxUser();
        string GetName();
        bool IsInRole(string role);
        bool IsRoleAtLeast(string role);

        Principal Authenticate(string username, string password);
        long GetPrincipalId(string username);

        long GetIdentityId();
        string GetUserAgent();
    }

    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor context;
        private readonly AuthContext authContext;
        private AmDbContext ctx;

        public IdentityService(IHttpContextAccessor context, AuthContext authContext, AmDbContext ctx = null)
        {
            this.context = context;
            this.authContext = authContext;
            this.ctx = ctx ?? new AmDbContext();
        }

        public string GetUserAgent()
        {
            string userAgent = "";
            if (context.HttpContext.Request.Headers.TryGetValue("User-Agent", out var headers))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var header in headers)
                {
                    sb.Append(header);
                    sb.Append(" ");
                }

                userAgent = sb.ToString().Trim();
            }
            return userAgent;
        }
        private ClaimsPrincipal User { get => context.HttpContext.User; }
        public ClaimsPrincipal GetUser()
        {
            return User;
        }
        public string GetSxUser()
        {
            //return User?.Claims.Where(x => x.Type == "sx.user").FirstOrDefault()?.Value;
            return User?.Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public bool IsRoleAtLeast(string role)
        {
            return false;
        }
        
        public string GetName()
        {
            return GetSxUser();
        }

        public long GetPrincipalId(string username)
        {
            return ctx.Principal
                .Where(x => x.Username == username)
                .FirstOrDefault()?.Id ?? 0;
        }

        public long GetIdentityId()
        {
            string id = User?.Claims.Where(x => x.Type == "id").FirstOrDefault()?.Value;
            return Convert.ToInt64(id);
        }

        public Principal Authenticate(string username, string password)
        {

            var principal = ctx.Principal
                .Where(x => x.Username == username && x.Password == password).FirstOrDefault();

            if (principal == null)
                throw new Exception($"Principal not found, {username}");
            if (principal.Role != Role.ADMIN && !principal.CompanyId.HasValue)
                throw new Exception($"Principal not valid, {username}");

            //if (new string[] { Role.MANAGER, Role.SALESMAN, Role.OPERATOR }.Contains(principal.Role) &&
            //    !principal.PrincipalGrouping.Select(x => x.Grouping).Any())
            //    throw new Exception($"Principal without group, {username}");

            //principal.AgencyIds = principal.PrincipalAuth.Where(x => x.AgencyId != null).Select(x => x.AgencyId.Value)
            //        .Distinct().ToList();

            //principal.ProductIds = principal.PrincipalAuth.Where(x => x.ProductId != null).Select(x => x.ProductId.Value)
            //    .Concat(principal.PrincipalGrouping.Select(x => x.Grouping).SelectMany(x => x.GroupingAuth).Where(x => x.ProductId != null).Select(x => x.ProductId.Value))
            //    .Distinct().ToList();
            var token = generateJwtToken(principal);
            principal.Token = token;
            return principal;
        }

        //public AuthenticateResponse Authenticate(string username, string password)
        //{
        //    var user = ctx.Principal.SingleOrDefault(x => x.Username == username && x.Password == password);

        //    // return null if user not found
        //    if (user == null) return null;

        //    // authentication successful so generate jwt token
        //    var token = generateJwtToken(user);

        //    return new AuthenticateResponse(user, token);
        //}        
        
        //public long GetAuthorizationFromHeader()
        //{
        //    if (context.HttpContext.Request.Headers.TryGetValue("sx-auth-id", out var idAutorizzazione))
        //    {
        //        return long.Parse(idAutorizzazione);
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid authorization header");
        //    }
        //}

        public class ClientFilter
        {

        }

        public class AutorizzazioneRisorsa
        {
            public string value { get; set; }
            public string description { get; set; }
        }

        #region PRIVATE token
        private string generateJwtToken(Principal user)
        {
            //var claims = new[] {
            //        new Claim("name", "callbackuser"),
            //        new Claim(ClaimTypes.NameIdentifier, "callbackuser"),
            //        new Claim(ClaimTypes.Name, "callbackuser"),
            //    };
            //var identity = new ClaimsIdentity(claims, Scheme.Name);
            
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(authContext.JWTSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[] { 
                        new Claim("id", user.Id.ToString()), 
                        new Claim("name", user.Name),
                        new Claim("username", user.Username),
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }

    public class IdentityMockService : IIdentityService
    {
        private readonly string id;
        private readonly string userAgent;

        public IdentityMockService(string id, string userAgent)
        {
            this.id = id;
            this.userAgent = userAgent;
        }

        public ClaimsPrincipal GetUser()
        {
            return new ClaimsPrincipal();
        }

        public string GetName()
        {
            return id;
        }

        public string GetUserAgent()
        {
            return userAgent;
        }


        public bool IsInRole(string ruolo)
        {
            return false;
        }

        public long GetIdentityId()
        {
            throw new NotImplementedException();
        }
        public string GetSxUser()
        {
            throw new NotImplementedException();
        }

        public Principal Authenticate(string username, string password)
        {
            //var user = ctx.Principal.SingleOrDefault(x => x.Username == username && x.Password == password);
            return new Principal()
            {
                //IdPartizione = 0,
                //Id = 76,
                Username = username
            };
        }

        public bool IsRoleAtLeast(string ruolo)
        {
            throw new NotImplementedException();
        }

        public long GetPrincipalId(string username)
        {
            throw new NotImplementedException();
        }

    }

    public class Identity
    {
        public long PrincipalId { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public long CompanyId { get; set; }

    }
}
