using System;
using Gaia.IdP.Message.Common;

namespace Gaia.IdP.Message.Filters
{
    public class GetActivityLogsFilter : IFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string UserName { get; set; }
        
    }
}