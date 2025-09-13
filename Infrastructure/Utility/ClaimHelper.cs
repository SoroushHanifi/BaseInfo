using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Infrastructure.Utility
{
    public class ClaimHelper : IClaimHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetUserInfo()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public string GetUserId()
        {
            if (_httpContextAccessor?.HttpContext != null)
            {
                var claimIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                var userId = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier).Value;

                return userId;
            }
            return string.Empty;
        }

        private List<Claim> GetUserClaims()
        {
            return _httpContextAccessor.HttpContext.User.Claims.ToList();
        }

        public List<string> GetAllRoleIds()
        {
            var claims = GetUserClaims();
            var jsonStirngRoleId = claims.Where(x => x.Type == "client_RoleIds").Select(x => x.Value).FirstOrDefault();
            var roleIds = jsonStirngRoleId != null ? JsonConvert.DeserializeObject<List<string>>(jsonStirngRoleId) : null;
            return roleIds;
        }

        public string GetToken()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            return token;
        }

        public string GetTokenFromCookie(bool byKey = true, string cookieName = null)
        {
            cookieName ??= ConstantValuesInfra.CookieName;
            var token = _httpContextAccessor.HttpContext.Request.Cookies[cookieName];
            return byKey ? $"{cookieName}={token}" : token;
        }

        public List<string> GetRoleCodes()
        {
            var claims = GetUserClaims();
            var jsonStringCode = claims.Where(x => x.Type == "applicationRoleOrganizationAreaChart").Select(c => c.Value).ToList();
            return jsonStringCode;
        }

        public string GetRoleType()
        {
            var claims = GetUserClaims();
            var jsonStringCode = claims.FirstOrDefault(x => x.Type == "roleType").Value;
            //var code = JsonConvert.DeserializeObject<string>(jsonStringCode);
            return jsonStringCode;
        }

        public List<string> GetUserRoleCode()
        {
            var claims = GetUserClaims();
            var jsonStringCode = claims.Where(x => x.Type == "applicationRoleOrganizationAreaChart").Select(c => c.Value).ToList();
            return jsonStringCode;
        }

        public IPAddress GetUserIP()
        {
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            return ip;
        }

        public string GetUserName()
        {
            var claims = GetUserClaims();
            var userName = claims.Where(x => x.Type.Equals(ConstantValuesInfra.UsernameClaim, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Value)
                .FirstOrDefault();
            return userName;
        }

        public DecodedBase64 BasicAuthContractors()
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers.Authorization.Where(p => p.ToLower().StartsWith("basic")).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(auth))
                return null;

            auth = auth.Split(" ").LastOrDefault();
            if (string.IsNullOrWhiteSpace(auth))
                return null;

            return GetBase64String(auth);
        }

        public DecodedBase64 GetBase64String(string encodedString)
        {
            var data = Convert.FromBase64String(encodedString);
            var decodedString = System.Text.Encoding.UTF8.GetString(data);

            var decodedSplit = decodedString.Split(':');
            return new DecodedBase64
            {
                Username = decodedSplit.FirstOrDefault(),
                Code = decodedSplit.LastOrDefault()
            };
        }
    }

    public class DecodedBase64
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }
}
