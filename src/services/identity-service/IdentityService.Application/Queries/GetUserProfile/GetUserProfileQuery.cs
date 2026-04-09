using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Queries.GetUserProfile
{
    /// <summary>
    /// Gets the current user's profile. The UserId comes from the JWT token (set by the controller).
    /// </summary>
    public record GetUserProfileQuery(Guid UserId) : IRequest<ApiResponse<UserProfileResponse>>;
}
