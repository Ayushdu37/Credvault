using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public string? DeviceInfo { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        // Computed properties (not stored in DB, calculated on the fly)
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsExpired && !IsRevoked;

        // Navigation property
        public User User { get; private set; } = null!;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, int expiryDays = 7, string? deviceInfo = null)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                DeviceInfo = deviceInfo,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            };
        }
        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
