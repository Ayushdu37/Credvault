using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Entities
{
    public class Bill
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid CardId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal MinimumDue { get; private set; }
        public decimal AmountPaid { get; private set; }
        public DateTime DueDate { get; private set; }
        public string BillingMonth { get; private set; } = string.Empty;
        public string Status { get; private set; } = "Pending";
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public ICollection<PaymentSchedule> PaymentSchedules { get; private set; } = [];

        private Bill() { }

        public static Bill Create(
        Guid userId, Guid cardId, decimal totalAmount,
        decimal minimumDue, DateTime dueDate, string billingMonth)
        {
            return new Bill
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CardId = cardId,
                TotalAmount = totalAmount,
                MinimumDue = minimumDue,
                AmountPaid = 0,
                DueDate = dueDate,
                BillingMonth = billingMonth,
                Status = "Pending",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }
        public void ApplyPayment(decimal amount)
        {
            AmountPaid += amount;
            Status = AmountPaid >= TotalAmount ? "Paid" : "PartiallyPaid";
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkOverdue()
        {
            if (Status != "Paid") Status = "Overdue";
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal Remaining => TotalAmount - AmountPaid;
        public bool IsPaid => Status == "Paid";
        public bool IsOverdue => !IsPaid && DueDate < DateTime.UtcNow;
    }
}
