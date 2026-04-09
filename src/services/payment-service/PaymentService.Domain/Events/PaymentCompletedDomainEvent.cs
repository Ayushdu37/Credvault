using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Events
{
    public record PaymentCompletedDomainEvent(
    Guid PaymentId, Guid UserId, Guid BillId, Guid CardId, decimal Amount);
}
