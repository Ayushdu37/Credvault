using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Billing.Requests
{
    public class SchedulePaymentRequest
    {
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
