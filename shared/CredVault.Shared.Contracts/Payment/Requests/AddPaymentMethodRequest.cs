using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Payment.Requests
{
    public class AddPaymentMethodRequest
    {
        public int MethodType { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
