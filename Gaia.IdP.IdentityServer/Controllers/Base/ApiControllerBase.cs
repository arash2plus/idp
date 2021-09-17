using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        //protected string UserId
        //{
        //    get
        //    {
        //        if (User.Identity is ClaimsIdentity s)
        //        {
        //            var userId = s.Claims.FirstOrDefault(i => i.Type.Equals("userId"));
        //            if (userId != null)
        //            {
        //                return userId.Value;
        //            }
        //            throw new Exception("Invalid Token");
        //        }
        //        throw new Exception("Invalid Token");
        //    }
        //}
    }
}