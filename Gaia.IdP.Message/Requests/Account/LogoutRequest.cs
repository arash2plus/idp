using System.Security.Claims;
using Gaia.IdP.Message.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.Message.Requests
{
    public class LogoutRequest : IRequest<LogoutResponse>
    {
        public string LogoutId { get; set; }

        public ClaimsPrincipal User { get; set; }

        public HttpRequest HttpRequest { get; set; }
    }
}
