using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Entities
{
    public class RewardTier
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int MinPoints { get; private set; }
        public decimal CashbackPercent { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation
        public ICollection<RewardAccount> RewardAccounts { get; private set; } = [];

        private RewardTier() { }

        public static RewardTier Create(
        Guid id, string name, int minPoints, decimal cashbackPercent)
        {
            return new RewardTier
            {
                Id = id,
                Name = name,
                MinPoints = minPoints,
                CashbackPercent = cashbackPercent,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
