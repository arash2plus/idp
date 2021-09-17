using Gaia.IdP.IdentityServer.Interfaces;
using Gaia.IdP.IdentityServer.Models;
using Gaia.IdP.IdentityServer.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Services
{
    public class GoogleRecaptchaVerificationService : ICaptchaVerificationService
    {
        private string _clientKey;
        private string _serverKey;
        private ILogger<GoogleRecaptchaVerificationService> _logger;

        public GoogleRecaptchaVerificationService(CaptchaSettingsOptions captchaSettingsOptions, ILogger<GoogleRecaptchaVerificationService> logger)
        {
            _serverKey = captchaSettingsOptions.ServerKey;
            _clientKey = captchaSettingsOptions.ClientKey;
            _logger = logger;

        }

        public async Task<bool> Verify(string captchaToken)
        {
            var result = false;

            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

            try
            {
                using var client = new HttpClient();

                var response = await client.GetAsync($"{googleVerificationUrl}?secret={_serverKey}&response={captchaToken}");
                var jsonString = await response.Content.ReadAsStringAsync();
                var captchaVerfication = JsonConvert.DeserializeObject<CaptchaVerification>(jsonString);

                result = captchaVerfication.Success;
            }
            catch (Exception e)
            {
                // fail gracefully, but log
                _logger.LogError("Failed to process captcha validation", e);
            }
            
            return result;
        }
    }
}
