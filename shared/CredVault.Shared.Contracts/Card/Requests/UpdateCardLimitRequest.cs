using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Card.Requests
{
    public class UpdateCardLimitRequest
    {
        public decimal NewCreditLimit { get; set; }
    }
}
