using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Responses
{
    /// <summary>
    /// Returned after successful login or token refresh.
    /// </summary>
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
