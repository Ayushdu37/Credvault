using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.SendOTP
{
    public record SendOTPCommand(string Email, OTPPurpose Purpose) : IRequest<ApiResponse<string>>;
}
