using Gaia.MessageBus.IntegrationEvents;

namespace Gaia.IdP.Message.Events
{
    public class PhoneNumberConfirmationTokenGeneratedEvent : Event
    {
        public string Receipient { get; set; }

        public string Token { get; set; }
    }
}
