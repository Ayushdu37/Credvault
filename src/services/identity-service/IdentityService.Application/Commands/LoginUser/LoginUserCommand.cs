using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.LoginUser
{
    public record LoginUserCommand(
    string Email,
    string Password,
    string? DeviceInfo
) : IRequest<ApiResponse<AuthResponse>>;
}
