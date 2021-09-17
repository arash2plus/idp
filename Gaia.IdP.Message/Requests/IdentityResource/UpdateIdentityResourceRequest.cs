using Gaia.IdP.Message.Commands;
using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class UpdateIdentityResourceRequest : UpdateIdentityResourceCommand, IRequest
    {
        public int Id { get; set; }
    }
}
