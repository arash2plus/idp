using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class GetUserProfileRequest : IRequest<UserProfile>
    {
        public string UserId { get; set; }
    }
}
