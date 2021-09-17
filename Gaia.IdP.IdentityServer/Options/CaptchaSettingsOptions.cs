using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Options
{
    public class CaptchaSettingsOptions : OptionsBase,IRegisterableOptions
    {

        public string ClientKey { get; set; }
        public string ServerKey { get; set; }

        public CaptchaSettingsOptions(IConfiguration configuration) : base("CaptchaSettings", configuration)
        {

        }


        protected override void Bind(IConfiguration configuration) => configuration.Bind(SectionKey, this);

    }
}
