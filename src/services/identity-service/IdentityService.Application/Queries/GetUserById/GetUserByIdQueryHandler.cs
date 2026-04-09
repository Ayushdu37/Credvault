using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserProfileResponse>>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserProfileResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return ApiResponse<UserProfileResponse>.FailureResponse("User not found.");
            return ApiResponse<UserProfileResponse>.SuccessResponse(new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
