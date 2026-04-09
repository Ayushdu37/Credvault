using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.VerifyEmail
{
    public record VerifyEmailCommand(string Email, string OTPCode) : IRequest<ApiResponse<string>>;
}
