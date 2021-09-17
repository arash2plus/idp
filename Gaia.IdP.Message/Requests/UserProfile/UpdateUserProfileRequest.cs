using Gaia.IdP.Message.Commands;
using Gaia.IdP.Message.Responses;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class UpdateUserProfileRequest : UpdateUserProfileCommand, IRequest<UserProfile>
    {
        public string UserId { get; set; }
    }
}
