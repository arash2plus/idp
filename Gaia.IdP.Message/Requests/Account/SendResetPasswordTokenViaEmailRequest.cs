using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class SendResetPasswordTokenViaEmailRequest : IRequest
    {
        public string Email { get; set; }
    }
}
