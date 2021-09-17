using System.ComponentModel.DataAnnotations;
using Gaia.IdP.Message.ValidationAttributes;

namespace Gaia.IdP.Message.Commands
{
    public class LoginViaOtpCommand
    {
        [PhoneNumberValidation, Required]
        public string PhoneNumber { get; set; }
        
        [Required]
        public string Token { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
}
