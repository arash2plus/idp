namespace Gaia.IdP.Message.Responses
{
    public class LogoutResponse
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
        public string LogoutId { get; set; }
    }
}