using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Abstraction
{
    /// <summary>
    /// Publishes events to RabbitMQ via MassTransit.
    /// Generic — can publish any event type.
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    }
}
