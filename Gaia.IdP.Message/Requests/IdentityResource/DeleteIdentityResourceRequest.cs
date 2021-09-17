using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class DeleteIdentityResourceRequest : IRequest
    {
        public int Id { get; set; }
    }
}
