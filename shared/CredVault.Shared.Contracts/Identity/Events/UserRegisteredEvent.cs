using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Events
{
    /// <summary>
    /// Published when a new user successfully registers.
    /// The Notification Service will pick this up and send a welcome email.
    /// </summary>
    public record UserRegisteredEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }
}
