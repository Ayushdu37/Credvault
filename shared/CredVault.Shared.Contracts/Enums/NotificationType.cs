using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    public enum NotificationType
    {
        PaymentSuccess = 0,
        PaymentFailed = 1,
        BillGenerated = 2,
        BillOverdue = 3,
        RewardEarned = 4,
        RewardRedeemed = 5,
        General = 6
    }
}
