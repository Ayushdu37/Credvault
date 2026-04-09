using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Responses
{
    public class RewardAccountResponse
    {
        public Guid Id { get; set; }
        public string TierName { get; set; } = string.Empty;
        public decimal CashbackPercent { get; set; }
        public int AvailablePoints { get; set; }
        public int TotalEarned { get; set; }
        public int PointsToNextTier { get; set; }
        public string NextTierName { get; set; } = string.Empty;
    }
}
