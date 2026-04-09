using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Events
{
    public record BillOverdueEvent
    {
        public Guid BillId { get; init; }
        public Guid UserId { get; init; }
        public Guid CardId { get; init; }
        public decimal TotalAmount { get; init; }
        public decimal AmountPaid { get; init; }
        public DateTime DueDate { get; init; }
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
