using Microsoft.Extensions.Configuration;

namespace Gaia.IdP.IdentityServer.Options
{
    public class SwaggerDocOptions : OptionsBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public SwaggerDocContactOptions Contact { get; set; }
        
        public SwaggerDocOptions(IConfiguration configuration): base("SwaggerDoc", configuration)
        {
        }

        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);
    }

    public class SwaggerDocContactOptions
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}
