using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Entities
{
    public class NotificationPreference
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public bool EmailEnabled { get; private set; } = true;
        public bool PaymentAlerts { get; private set; } = true;
        public bool BillReminders { get; private set; } = true;
        public bool RewardUpdates { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private NotificationPreference() { }

        public static NotificationPreference CreateDefault(Guid userId)
        {
            return new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EmailEnabled = true,
                PaymentAlerts = true,
                BillReminders = true,
                RewardUpdates = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void Update(bool emailEnabled, bool paymentAlerts,
        bool billReminders, bool rewardUpdates)
        {
            EmailEnabled = emailEnabled;
            PaymentAlerts = paymentAlerts;
            BillReminders = billReminders;
            RewardUpdates = rewardUpdates;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
