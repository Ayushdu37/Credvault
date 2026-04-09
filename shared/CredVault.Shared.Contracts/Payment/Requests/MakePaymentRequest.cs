using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Payment.Requests
{
    public class MakePaymentRequest
    {
        public Guid BillId { get; set; }
        public Guid CardId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
    }
}
