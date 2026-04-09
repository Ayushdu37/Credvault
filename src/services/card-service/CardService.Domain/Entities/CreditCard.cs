using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Entities
{
    public class CreditCard
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string MaskedNumber { get; private set; } = string.Empty;
        public string CardNumberHash { get; private set; } = string.Empty;
        public string CardHolderName { get; private set; } = string.Empty;
        public int ExpiryMonth { get; private set; }
        public int ExpiryYear { get; private set; }
        public Guid IssuerId { get; private set; }
        public decimal CreditLimit { get; private set; }
        public decimal OutstandingBalance { get; private set; }
        public int BillingCycleStartDay { get; private set; }
        public bool IsDefault { get; private set; }
        public bool IsVerified { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation property
        public CardIssuer Issuer { get; private set; } = null!;

        // Private constructor for EF Core
        private CreditCard() { }

        /// <summary>
        /// Factory method — the ONLY way to create a CreditCard.
        /// maskedNumber = e.g. "**** **** **** 1234"
        /// cardNumberHash = SHA256 hash for duplicate detection
        /// </summary>
        public static CreditCard Create(
        Guid userId,
        string maskedNumber,
        string cardNumberHash,
        string cardHolderName,
        int expiryMonth,
        int expiryYear,
        Guid issuerId,
        decimal creditLimit,
        int billingCycleStartDay)
        {
            return new CreditCard
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MaskedNumber = maskedNumber,
                CardNumberHash = cardNumberHash,
                CardHolderName = cardHolderName,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                IssuerId = issuerId,
                CreditLimit = creditLimit,
                OutstandingBalance = 0,
                BillingCycleStartDay = billingCycleStartDay,
                IsDefault = false,
                IsVerified = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        // --- State change methods ---
        public void SetAsDefault()
        {
            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UnsetDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Verify()
        {
            IsVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCreditLimit(decimal newLimit)
        {
            CreditLimit = newLimit;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOutstandingBalance(decimal amount)
        {
            OutstandingBalance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        // --- Computed properties ---
        public decimal AvailableCredit => CreditLimit - OutstandingBalance;
        public bool IsExpired =>
            new DateTime(ExpiryYear, ExpiryMonth, 1).AddMonths(1) <= DateTime.UtcNow;
        public bool IsExpiringSoon =>
            !IsExpired && new DateTime(ExpiryYear, ExpiryMonth, 1).AddMonths(1)
                <= DateTime.UtcNow.AddDays(30);
    }
}
