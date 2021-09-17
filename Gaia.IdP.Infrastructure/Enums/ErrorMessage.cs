namespace Gaia.IdP.Infrastructure.Enums
{
    public enum ErrorMessage
    {
        // bad request (400 series) client error details
        userNotFound,
        duplicateUserName,
        duplicateEmail,
        duplicatePhoneNumber,
        invalidEmail,
        emptyPhoneNumber,
        invalidPhoneNumber,
        alreadyConfirmed,
        identityResourceNotFound,
        apiScopeNotFound,
        apiResourceNotFound,
        clientNotFound,
        entityWithTheSameKeyAlreadyExists,

        // unauthorized (401 series) client error details
        invalidUsernameOrPassword,
        tokenNotVerified,
        captchaTokenNotVerified,

        // forbidden (403 series) client error details
        requiresTwoFactor,
        phoneNumberNotConfirmed,
        emailNotConfirmed,
        isLockedOut
    }
}