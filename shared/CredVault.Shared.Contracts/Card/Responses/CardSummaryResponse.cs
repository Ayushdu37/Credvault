using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Responses
{
    public class CardSummaryResponse
    {
        public int TotalCards { get; set; }
        public decimal TotalCreditLimit { get; set; }
        public decimal TotalOutstandingBalance { get; set; }
        public decimal TotalAvailableCredit { get; set; }
        public decimal UtilizationPercentage { get; set; }
    }
}
