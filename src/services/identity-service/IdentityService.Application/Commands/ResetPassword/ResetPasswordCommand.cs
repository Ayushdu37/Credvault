using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.ResetPassword
{
    public record ResetPasswordCommand(
    string Email,
    string OTPCode,
    string NewPassword
) : IRequest<ApiResponse<string>>;
}
