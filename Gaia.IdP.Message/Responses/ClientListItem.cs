namespace Gaia.IdP.Message.Responses
{
    public class ClientListItem
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
    }
}