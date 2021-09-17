using System.ComponentModel.DataAnnotations;

namespace Gaia.IdP.Message.Commands
{
    public class ResetPasswordViaTokenInEmailCommand
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Token { get; set; }
        
        [Required]
        public string NewPassword { get; set; }
    }
}
