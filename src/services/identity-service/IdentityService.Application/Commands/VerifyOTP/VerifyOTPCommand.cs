using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.VerifyOTP
{
    public record VerifyOTPCommand(string Email, string OTPCode, OTPPurpose Purpose) : IRequest<ApiResponse<string>>;
}
