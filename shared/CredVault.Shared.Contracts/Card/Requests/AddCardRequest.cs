using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Requests
{
    public class AddCardRequest
    {
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public CardIssuer Issuer { get; set; }
        public string? Nickname { get; set; }
        public decimal CreditLimit { get; set; }
        public int BillingCycleStartDay { get; set; }
    }
}
