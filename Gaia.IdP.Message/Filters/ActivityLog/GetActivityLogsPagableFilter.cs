using Gaia.IdP.Message.Common;

namespace Gaia.IdP.Message.Filters
{
    public class GetActivityLogsPagableFilter : GetActivityLogsFilter, IPagableFilter
    {
        public int? Skip { get; set; }
        public int? Limit { get; set; }
        public string Sort { get; set; }
    }
}