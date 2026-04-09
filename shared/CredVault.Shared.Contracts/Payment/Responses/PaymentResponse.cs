using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Payment.Responses
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid CardId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
