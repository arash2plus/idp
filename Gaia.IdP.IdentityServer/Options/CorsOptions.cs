using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class CorsOptions : OptionsBase
    {
        public IEnumerable<string> AllowedOrigins { get; set; }
        
        public CorsOptions(IConfiguration configuration): base("Cors", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }
}
