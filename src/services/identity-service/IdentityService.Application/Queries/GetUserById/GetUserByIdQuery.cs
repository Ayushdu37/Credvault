using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<ApiResponse<UserProfileResponse>>;
}
