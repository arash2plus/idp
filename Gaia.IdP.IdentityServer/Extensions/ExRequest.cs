using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.IdentityServer.Extensions
{
    public static class ExRequest
    {
        public static string GetUserName(this HttpRequest request)
        {
            var claim = GetClaim(request, ClaimTypes.Name);
            return claim?.Value;
        }

        public static string GetUserId(this HttpRequest request)
        {
            var claim = GetClaim(request, "sub");
            return claim?.Value;
        }
        
        public static IEnumerable<string> GetUserRoles(this HttpRequest request)
        {
            var claim = GetClaim(request, ClaimTypes.Role);
            return claim?.Value.Split(',').Select(o => o.Trim());
        }

        private static Claim GetClaim(this HttpRequest request , string claimType)
        {
            var identityClaims = request.HttpContext.User.Identity as ClaimsIdentity;
            return identityClaims?.FindFirst(claimType);
        }
    }
}
