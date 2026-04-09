using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Entities
{
    public class RewardTransaction
    {
        public Guid Id { get; private set; }
        public Guid RewardAccountId { get; private set; }
        public Guid? PaymentId { get; private set; }
        public string Type { get; private set; } = string.Empty;
        public int Points { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation
        public RewardAccount RewardAccount { get; private set; } = null!;

        private RewardTransaction() { }

        public static RewardTransaction CreateEarned(
            Guid rewardAccountId, Guid paymentId, int points, string description)
        {
            return new RewardTransaction
            {
                Id = Guid.NewGuid(),
                RewardAccountId = rewardAccountId,
                PaymentId = paymentId,
                Type = "Earned",
                Points = points,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static RewardTransaction CreateRedeemed(
        Guid rewardAccountId, int points, string description)
        {
            return new RewardTransaction
            {
                Id = Guid.NewGuid(),
                RewardAccountId = rewardAccountId,
                Type = "Redeemed",
                Points = -points,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
