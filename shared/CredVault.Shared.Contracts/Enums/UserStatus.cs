using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    /// <summary>
    /// Lifecycle of a user account:
    ///   PendingVerification → Active → (optionally Suspended/Deactivated)
    /// </summary>
    public enum UserStatus
    {
        PendingVerification = 0,  // Just registered, hasn't verified email yet
        Active = 1,               // Email verified, good to go
        Suspended = 2,            // Temporarily blocked (e.g., suspicious activity)
        Deactivated = 3           // User chose to close their account
    }
}
