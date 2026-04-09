using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Events
{
    /// <summary>
    /// Published when a user successfully adds a new credit card.
    /// </summary>
    public record CardAddedEvent
    {
        public Guid CardId { get; init; }
        public Guid UserId { get; init; }
        public string CardNickname { get; init; } = string.Empty;
        public string IssuerName { get; init; } = string.Empty;
        public string Last4Digits { get; init; } = string.Empty;
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
