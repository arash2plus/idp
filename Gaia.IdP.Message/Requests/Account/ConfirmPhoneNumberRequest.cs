using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class ConfirmPhoneNumberRequest : IRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
