using Gaia.IdP.Message.Common;

namespace Gaia.IdP.Message.Filters
{
    public class GetClientsFilter : IFilter
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
