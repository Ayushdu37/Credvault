using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Notification.Requests
{
    public class UpdatePreferencesRequest
    {
        public bool EmailEnabled { get; set; } = true;
        public bool PaymentAlerts { get; set; } = true;
        public bool BillReminders { get; set; } = true;
        public bool RewardUpdates { get; set; } = true;
    }
}
