using BillingService.Application.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Messaging.Publishers
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public EventPublisher(IPublishEndpoint publishEndpoint)
            => _publishEndpoint = publishEndpoint;
        public async Task PublishAsync<T>(T @event, CancellationToken ct = default)
            where T : class
            => await _publishEndpoint.Publish(@event, ct);
    }
}
