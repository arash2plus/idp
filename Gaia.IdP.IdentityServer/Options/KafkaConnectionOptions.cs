using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class KafkaConnectionOptions : OptionsBase
    {
        public IEnumerable<string> Brokers { get; set; }
        public uint DefaultMillisecondsTimeOut { get; set; } = 1000;
        
        public KafkaConnectionOptions(IConfiguration configuration): base("KafkaConnection", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }
}
