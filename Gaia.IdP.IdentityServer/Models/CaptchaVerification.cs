using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Models
{
    public class CaptchaVerification
    {
        public CaptchaVerification()
        {
        }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public IList Errors { get; set; }
    }
}
