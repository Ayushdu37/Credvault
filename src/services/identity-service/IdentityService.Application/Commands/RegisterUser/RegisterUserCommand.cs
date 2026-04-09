using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.RegisterUser
{
    /// <summary>
    /// IRequest<T> tells MediatR: "this command returns ApiResponse<string>"
    /// </summary>
    public record RegisterUserCommand(
    string Email,
    string Password,
    string FullName,
    string PhoneNumber
) : IRequest<ApiResponse<string>>;
}
