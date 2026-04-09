using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Notification.Responses
{
    public class NotificationPreferenceResponse
    {
        public bool EmailEnabled { get; set; }
        public bool PaymentAlerts { get; set; }
        public bool BillReminders { get; set; }
        public bool RewardUpdates { get; set; }
    }
}
