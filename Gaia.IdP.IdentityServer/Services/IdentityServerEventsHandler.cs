using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Gaia.IdP.IdentityServer.Services
{
    public class IdentityServerEventsHandler : IEventSink
    {
        private readonly ILogger<IdentityServerEventsHandler> _logger;

        public IdentityServerEventsHandler(ILogger<IdentityServerEventsHandler> logger)
        {
            _logger = logger;
        }

        public Task PersistAsync(Event evt)
        {
            if (evt.EventType == EventTypes.Success || evt.EventType == EventTypes.Information)
            {
                _logger.LogInformation("{Name} ({Id}), Details: {@details}",
                    evt.Name,
                    evt.Id,
                    evt);
            }
            else
            {
                _logger.LogError("{Name} ({Id}), Details: {@details}",
                    evt.Name,
                    evt.Id,
                    evt);
            }

            return Task.CompletedTask;
        }
    }
}