using IdentityService.Application.Abstraction;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Messaging.Publishers
{
    /// <summary>
    /// Publishes events to RabbitMQ using MassTransit.
    /// Any service subscribed to this event type will receive it.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public EventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
        }
    }
}
