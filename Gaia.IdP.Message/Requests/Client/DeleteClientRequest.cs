using MediatR;

namespace Gaia.IdP.Message.Requests
{
    public class DeleteClientRequest : IRequest
    {
        public int Id { get; set; }
    }
}
