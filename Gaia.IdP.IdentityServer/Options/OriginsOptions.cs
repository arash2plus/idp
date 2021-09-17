using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class OriginsOptions : OptionsBase
    {
        public string IdP { get; set; }
        public string IdPClient { get; set; }
        
        public OriginsOptions(IConfiguration configuration): base("Origins", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }
}
