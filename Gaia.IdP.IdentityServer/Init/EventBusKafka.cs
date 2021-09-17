using Gaia.IdP.IdentityServer.Options;
using Gaia.MessageBus.EventBusKafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class EventBusKafka
    {
        public static IServiceCollection AddEventBusKafka(this IServiceCollection services, IConfiguration configuration)
        {
            var idpKafkaConnectionOptions = new KafkaConnectionOptions(configuration);

            var eventBusKafkaConnnectionOptions = new Gaia.MessageBus.EventBusKafka.Options.ConnectionOptions {
                Brokers = idpKafkaConnectionOptions.Brokers
            };
            
            services.AddEventBusKafka(eventBusKafkaConnnectionOptions);

            return services;
        }
    }
}
