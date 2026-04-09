using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Enums
{
    /// <summary>
    /// Why was the OTP sent? This determines what happens after verification.
    /// </summary>
    public enum OTPPurpose
    {
        EmailVerification = 0,
        PasswordReset = 1,
        Login = 2
    }
}
