using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Events
{
    public record PaymentCreatedDomainEvent(
    Guid PaymentId, Guid UserId, Guid BillId, decimal Amount);
}
