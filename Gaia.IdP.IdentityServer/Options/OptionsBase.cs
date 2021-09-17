using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public abstract class OptionsBase
    {
        protected string SectionKey { get; }

        public OptionsBase(string sectionKey, IConfiguration configuration)
        {
            SectionKey = sectionKey;
            Bind(configuration);
        }

        protected abstract void Bind(IConfiguration configuration);
    }
}