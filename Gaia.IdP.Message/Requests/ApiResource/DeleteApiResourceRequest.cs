using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class DeleteApiResourceRequest : IRequest
    {
        public int Id { get; set; }
    }
}
