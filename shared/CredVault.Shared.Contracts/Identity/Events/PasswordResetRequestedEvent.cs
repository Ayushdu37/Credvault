using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Events
{
    /// <summary>
    /// Published when a user requests a password reset.
    /// Notification Service sends them the OTP code.
    /// </summary>
    public record PasswordResetRequestedEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string OTPCode { get; init; } = string.Empty;
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
