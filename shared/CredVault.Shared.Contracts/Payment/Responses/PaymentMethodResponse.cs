using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Payment.Responses
{
    public class PaymentMethodResponse
    {
        public Guid Id { get; set; }
        public string MethodType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
