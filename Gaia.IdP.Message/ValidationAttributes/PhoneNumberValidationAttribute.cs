using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gaia.IdP.Message.ValidationAttributes
{
    public sealed class PhoneNumberValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = (System.String)value;
            var pattern = "^[0][1-9]\\d{9}$";

            if (phoneNumber != null && !Regex.Match(phoneNumber, pattern).Success)
                return new ValidationResult("invalidPhoneNumber", new [] { validationContext.MemberName });

            return ValidationResult.Success;
        }
    }

    public sealed class PhoneNumbersValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumbers = ((System.String)value)?.Split(',').Select(o => o.Trim());
            var pattern = "^[0][1-9]\\d{9}$";

            if (phoneNumbers!= null && phoneNumbers.Any(phoneNumber => !Regex.Match(phoneNumber, pattern).Success))
                return new ValidationResult("invalidPhoneNumber", new [] { validationContext.MemberName });

            return ValidationResult.Success;
        }
    }
}