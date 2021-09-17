using System.ComponentModel.DataAnnotations;
using Gaia.IdP.Message.ValidationAttributes;

namespace Gaia.IdP.Message.Commands
{
    public class LoginCommand
    {
       
        [Required, PhoneNumberValidation]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ReturnUrl { get; set; }


        public string CaptchaToken { get; set; }

    }
}
