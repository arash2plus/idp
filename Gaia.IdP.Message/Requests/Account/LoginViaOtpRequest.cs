using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Gaia.IdP.Message.Requests
{
    public class LoginViaOtpRequest : LoginViaOtpCommand, IRequest
    {
        public HttpRequest HttpRequest { get; set; }
    }
}
