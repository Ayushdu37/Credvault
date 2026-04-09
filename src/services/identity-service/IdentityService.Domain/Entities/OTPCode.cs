using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Entities
{
    public class OTPCode
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public OTPPurpose Purpose { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Computed property
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsUsed && !IsExpired;

        // Navigation property
        public User User { get; private set; } = null!;

        private OTPCode() { }

        public static OTPCode Create(Guid userId, string code, OTPPurpose purpose, int expiryMinutes = 5)
        {
            return new OTPCode
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = code,
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
        }
        public void MarkUsed()
        {
            IsUsed = true;
        }
    }
}
