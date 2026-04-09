using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Notification.Events
{
    public record NotificationSentEvent
    {
        public Guid NotificationId { get; init; }
        public Guid UserId { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
