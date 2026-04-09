using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Events
{
    public record CardAddedDomainEvent(Guid CardId, Guid UserId, string MaskedNumber);
}
