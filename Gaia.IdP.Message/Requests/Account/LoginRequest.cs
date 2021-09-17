using Gaia.IdP.Message.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.Message.Requests
{
    public class LoginRequest : LoginCommand, IRequest
    {
        public HttpRequest HttpRequest { get; set; }
    }
}
