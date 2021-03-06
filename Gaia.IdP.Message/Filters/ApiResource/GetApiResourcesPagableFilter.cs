using Gaia.IdP.Message.Common;

namespace Gaia.IdP.Message.Filters
{
    public class GetApiResourcesPagableFilter : GetApiResourcesFilter, IPagableFilter
    {
        public int? Skip { get; set; }
        public int? Limit { get; set; }
        public string Sort { get; set; }
    }
}
