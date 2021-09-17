using System.Threading.Tasks;

namespace Gaia.IdP.IdentityServer.Interfaces
{
    public  interface ICaptchaVerificationService
    {
        public Task<bool> Verify(string token);
    }
}
