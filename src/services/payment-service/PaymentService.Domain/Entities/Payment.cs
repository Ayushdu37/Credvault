using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BillId { get; private set; }
        public Guid CardId { get; private set; }
        public decimal Amount { get; private set; }
        public string PaymentMethod { get; private set; } = string.Empty;
        public string? TransactionReference { get; private set; }
        public string Status { get; private set; } = "Processing";
        public string? FailureReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private Payment() { }

        /// <summary>
        /// Saga Step 1: Create payment in "Processing" state
        /// </summary>
        public static Payment Create(
        Guid userId, Guid billId, Guid cardId, decimal amount,
        string paymentMethod, string? transactionReference = null)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BillId = billId,
                CardId = cardId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                TransactionReference = transactionReference,
                Status = "Processing",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Saga Step 2 (success): Mark as Completed
        /// </summary>
        public void MarkCompleted()
        {
            Status = "Completed";
            CompletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Saga Compensation: Mark as Failed with reason
        /// </summary>
        public void MarkFailed(string reason)
        {
            Status = "Failed";
            FailureReason = reason;
        }

        /// <summary>Mark as Refunded</summary>
        public void MarkRefunded()
        {
            Status = "Refunded";
        }

        public bool IsProcessing => Status == "Processing";
        public bool IsCompleted => Status == "Completed";
    }
}
