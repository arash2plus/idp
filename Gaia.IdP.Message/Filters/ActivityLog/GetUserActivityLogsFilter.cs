using System;
using Gaia.IdP.Message.Common;

namespace Gaia.IdP.Message.Filters
{
    public class GetUserActivityLogsFilter : IFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}