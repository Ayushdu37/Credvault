using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Entities
{
    public class SavedPaymentMethod
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string MethodType { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public string Details { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private SavedPaymentMethod() { }

        public static SavedPaymentMethod Create(
        Guid userId, string methodType, string displayName, string details)
        {
            return new SavedPaymentMethod
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MethodType = methodType,
                DisplayName = displayName,
                Details = details,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void SetAsDefault() => IsDefault = true;
        public void UnsetDefault() => IsDefault = false;
    }
}
