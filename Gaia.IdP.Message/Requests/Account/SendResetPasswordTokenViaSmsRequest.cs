using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class SendResetPasswordTokenViaSmsRequest : IRequest
    {
        public string PhoneNumber { get; set; }
    }
}
