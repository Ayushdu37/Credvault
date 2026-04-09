using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Responses
{
    public class PaymentScheduleResponse
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduledDate { get; set; }
        public PaymentScheduleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
