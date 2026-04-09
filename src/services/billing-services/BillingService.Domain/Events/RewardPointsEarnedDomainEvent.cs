using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Events
{
    public record RewardPointsEarnedDomainEvent(Guid UserId, int Points, string TierName);
}
