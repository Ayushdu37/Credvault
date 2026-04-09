using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Responses
{
    public class CardResponse
    {
        public Guid Id { get; set; }
        public string MaskedNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public CardIssuer Issuer { get; set; }
        public string IssuerName { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal AvailableCredit { get; set; }
        public int BillingCycleStartDay { get; set; }
        public bool IsDefault { get; set; }
        public bool IsVerified { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
