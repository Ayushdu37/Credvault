using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Requests
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
