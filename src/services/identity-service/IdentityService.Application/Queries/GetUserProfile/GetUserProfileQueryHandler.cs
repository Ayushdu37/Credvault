using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ApiResponse<UserProfileResponse>>
    {
        private readonly IUserRepository _userRepository;
        public GetUserProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return ApiResponse<UserProfileResponse>.FailureResponse("User not found.");
            var response = new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            };
            return ApiResponse<UserProfileResponse>.SuccessResponse(response);
        }
    }
}
