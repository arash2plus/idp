using Gaia.MessageBus.IntegrationEvents;

namespace Gaia.IdP.Message.Events
{
    public class ResetPasswordEmailTokenGeneratedEvent : Event
    {
        public string ReceipientName { get; set; }
        
        public string ReceipientAddress { get; set; }

        public string Token { get; set; }
    }
}
