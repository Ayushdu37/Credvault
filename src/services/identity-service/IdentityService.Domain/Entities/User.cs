using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public UserRole Role { get; private set; } = UserRole.User;
        public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
        public bool IsEmailVerified { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties (EF Core will use these to link tables)
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
        public ICollection<OTPCode> OTPCodes { get; private set; } = [];

        // Private constructor — EF Core needs this to create objects from DB
        private User() { }

        /// <summary>
        /// Factory method — the ONLY way to create a new User.
        /// This ensures a User is never created in an invalid state.
        /// </summary>
        public static User Create(string email, string passwordHash, string fullName, string phoneNumber)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant(),   // Always store emails lowercase
                PasswordHash = passwordHash,
                FullName = fullName,
                PhoneNumber = phoneNumber
            };
        }

        // --- Methods to change the user's state ---
        public void VerifyEmail()
        {
            IsEmailVerified = true;
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Suspend()
        {
            Status = UserStatus.Suspended;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
