using System.ComponentModel.DataAnnotations;

namespace Gaia.IdP.Message.Commands
{
    public class ConfirmPhoneNumberCommand
    {
        [Required]
        public string Token { get; set; }
    }
}
