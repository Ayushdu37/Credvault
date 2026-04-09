using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Events
{
    public record CardExpirySoonDomainEvent(Guid CardId, Guid UserId, DateTime ExpiryDate);
}
