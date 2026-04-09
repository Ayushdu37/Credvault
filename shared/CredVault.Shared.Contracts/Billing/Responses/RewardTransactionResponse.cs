using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Responses
{
    public class RewardTransactionResponse
    {
        public Guid Id { get; set; }
        public RewardTransactionType Type { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
