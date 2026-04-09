using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Requests
{
    public class GenerateBillRequest
    {
        public Guid CardId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MinimumDue { get; set; }
        public DateTime DueDate { get; set; }
        public string BillingMonth { get; set; } = string.Empty;
    }
}
