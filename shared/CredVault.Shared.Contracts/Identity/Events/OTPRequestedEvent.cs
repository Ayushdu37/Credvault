using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Events
{
    /// <summary>
    /// Published when any OTP is generated (email verification, password reset, etc.)
    /// </summary>
    public record OTPRequestedEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string OTPCode { get; init; } = string.Empty;
        public OTPPurpose Purpose { get; init; }
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
