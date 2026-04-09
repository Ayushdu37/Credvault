using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    public enum BillStatus
    {
        Pending = 0,
        Paid = 1,
        Overdue = 2,
        PartiallyPaid = 3
    }
}
