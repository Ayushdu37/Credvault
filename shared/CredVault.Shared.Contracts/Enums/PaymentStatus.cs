using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    public enum PaymentStatus
    {
        Processing = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3
    }
}
