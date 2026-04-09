using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Events
{
    /// <summary>
    /// Raised internally when a new user is created.
    /// Different from the Shared.Contracts event — this one stays INSIDE the service.
    /// </summary>
    public record UserRegisteredDomainEvent(Guid UserId, string Email, string FullName);
}
