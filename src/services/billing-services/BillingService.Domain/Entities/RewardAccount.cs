using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Entities
{
    public class RewardAccount
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid TierId { get; private set; }
        public int AvailablePoints { get; private set; }
        public int TotalEarned { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public RewardTier Tier { get; private set; } = null!;
        public ICollection<RewardTransaction> Transactions { get; private set; } = [];

        private RewardAccount() { }

        public static RewardAccount Create(Guid userId, Guid defaultTierId)
        {
            return new RewardAccount
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TierId = defaultTierId,
                AvailablePoints = 0,
                TotalEarned = 0,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void EarnPoints(int points)
        {
            AvailablePoints += points;
            TotalEarned += points;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanRedeem(int points) => AvailablePoints >= points;

        public void RedeemPoints(int points)
        {
            AvailablePoints -= points;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTier(Guid newTierId)
        {
            TierId = newTierId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
