using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<ApiResponse<AuthResponse>>;
}
