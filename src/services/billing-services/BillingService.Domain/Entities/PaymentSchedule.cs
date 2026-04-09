using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Entities
{
    public class PaymentSchedule
    {
        public Guid Id { get; private set; }
        public Guid BillId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public string Status { get; private set; } = "Pending";
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public Bill Bill { get; private set; } = null!;

        private PaymentSchedule() { }

        public static PaymentSchedule Create(
        Guid billId, Guid userId, decimal amount, DateTime scheduledDate)
        {
            return new PaymentSchedule
            {
                Id = Guid.NewGuid(),
                BillId = billId,
                UserId = userId,
                Amount = amount,
                ScheduledDate = scheduledDate,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkExecuted()
        {
            Status = "Executed";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = "Cancelled";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
