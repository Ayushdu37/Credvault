using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Abstractions
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
    }
}
