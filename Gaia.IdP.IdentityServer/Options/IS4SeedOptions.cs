using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class IS4SeedOptions : OptionsBase
    {
        public bool Enabled { get; set; }
        public IEnumerable<IdentityResource> IdentityResources { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<ApiScope> ApiScopes { get; set; }
        public IEnumerable<ApiResource> ApiResources { get; set; }

        public IS4SeedOptions(IConfiguration configuration): base("IS4Seed", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }
}
