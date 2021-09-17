namespace Gaia.IdP.DomainModel.Customizations.Tokens
{
    public static class TokenPurposes
    {
        public static string OtpLogin { get { return "otp-login"; } }
        public static string PhoneNumberConfirmation { get { return "phone-number-confirmation"; } }
        public static string ResetPasswordViaSms { get { return "reset-password-via-sms"; } }
    }
}
