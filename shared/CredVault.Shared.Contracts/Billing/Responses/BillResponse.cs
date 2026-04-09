using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Responses
{
    public class BillResponse
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MinimumDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Remaining => TotalAmount - AmountPaid;
        public DateTime DueDate { get; set; }
        public string BillingMonth { get; set; } = string.Empty;
        public BillStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
