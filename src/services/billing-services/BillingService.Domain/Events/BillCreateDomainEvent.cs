using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Events
{
    public record BillCreatedDomainEvent(Guid BillId, Guid UserId, Guid CardId, decimal TotalAmount);
}
