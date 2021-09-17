using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class IS4UserInteractionOptoins: OptionsBase
    {
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string ErrorUrl { get; set; }
        
        public IS4UserInteractionOptoins(IConfiguration configuration): base("IS4UserInteraction", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }
}
