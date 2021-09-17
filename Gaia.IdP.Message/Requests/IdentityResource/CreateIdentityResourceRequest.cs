using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class CreateIdentityResourceRequest : CreateIdentityResourceCommand, IRequest
    {
    }
}
