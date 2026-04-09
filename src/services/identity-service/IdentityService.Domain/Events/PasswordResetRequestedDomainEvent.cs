using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Events
{
    public record PasswordResetRequestedDomainEvent(Guid UserId, string Email);
}
