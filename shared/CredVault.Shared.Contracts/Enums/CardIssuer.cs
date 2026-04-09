using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    /// <summary>
    /// Card network types. Matches CardIssuers table in card-service DB.
    /// </summary>
    public enum CardIssuer
    {
        Visa = 0,
        MasterCard = 1,
        Amex = 2,
        RuPay = 3,
        Discover = 4,
        DinersClub = 5
    }
}
