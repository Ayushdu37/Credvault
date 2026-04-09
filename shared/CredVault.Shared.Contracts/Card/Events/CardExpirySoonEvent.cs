using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Events
{
    /// <summary>
    /// Published when a credit card is expiring within 30 days.
    /// Notification Service sends a reminder email.
    /// </summary>
    public record CardExpirySoonEvent
    {
        public Guid CardId { get; init; }
        public Guid UserId { get; init; }
        public string Last4Digits { get; init; } = string.Empty;
        public DateTime ExpiryDate { get; init; }
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
