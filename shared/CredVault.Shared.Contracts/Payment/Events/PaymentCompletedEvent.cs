using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Payment.Events
{
    public record PaymentCompletedEvent
    {
        public Guid PaymentId { get; init; }
        public Guid UserId { get; init; }
        public Guid CardId { get; init; }
        public Guid BillId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
