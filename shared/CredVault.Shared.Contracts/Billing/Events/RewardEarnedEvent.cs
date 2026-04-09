using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Events
{
    public record RewardEarnedEvent
    {
        public Guid UserId { get; init; }
        public Guid PaymentId { get; init; }
        public int PointsEarned { get; init; }
        public int TotalPoints { get; init; }
        public string TierName { get; init; } = string.Empty;
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
