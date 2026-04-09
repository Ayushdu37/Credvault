using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponse>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the existing refresh token
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (existingToken is null || !existingToken.IsActive)
                return ApiResponse<AuthResponse>.FailureResponse("Invalid or expired refresh token.");
            // 2. Get the user
            var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
            if (user is null)
                return ApiResponse<AuthResponse>.FailureResponse("User not found.");
            // 3. Revoke the old token (token rotation — each refresh token is single-use)
            await _refreshTokenRepository.RevokeAsync(existingToken, cancellationToken);
            // 4. Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
            var newRefreshToken = Domain.Entities.RefreshToken.Create(
                user.Id, newRefreshTokenValue, deviceInfo: existingToken.DeviceInfo);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            return ApiResponse<AuthResponse>.SuccessResponse(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            }, "Token refreshed successfully.");
        }
    }
}
