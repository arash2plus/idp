using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gaia.IdP.Message.ValidationAttributes;

namespace Gaia.IdP.Message.Commands
{
    public class RegisterAccountCommand
    {
        [PhoneNumberValidation, Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Image { get; set; }

        public DateTime? Birthdate { get; set; }

        public string NationalId { get; set; }

        public string FatherName { get; set; }
    }
}
