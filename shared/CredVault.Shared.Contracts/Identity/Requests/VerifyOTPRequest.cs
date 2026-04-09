using CredVault.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Identity.Requests
{
    public class VerifyOTPRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OTPCode { get; set; } = string.Empty;
        public OTPPurpose Purpose { get; set; }
    }
}
