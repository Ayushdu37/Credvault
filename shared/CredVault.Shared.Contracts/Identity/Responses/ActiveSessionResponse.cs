using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Responses
{
    public class ActiveSessionResponse
    {
        public Guid TokenId { get; set; }
        public string? DeviceInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
