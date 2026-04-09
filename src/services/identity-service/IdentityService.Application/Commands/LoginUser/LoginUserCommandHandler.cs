using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using CredVault.Shared.Contracts.Identity.Responses;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the user
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return ApiResponse<AuthResponse>.FailureResponse("Invalid email or password.");

            // 2. Verify password
            if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
                return ApiResponse<AuthResponse>.FailureResponse("Invalid email or password.");

            // 3. Check if account is active
            if (user.Status != UserStatus.Active)
                return ApiResponse<AuthResponse>.FailureResponse($"Account is {user.Status}. Please verify your email first.");

            // 4. Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            // 5. Store the refresh token
            var refreshToken = Domain.Entities.RefreshToken.Create(user.Id, refreshTokenValue, deviceInfo: request.DeviceInfo);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            return ApiResponse<AuthResponse>.SuccessResponse(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15) // Match the token expiry
            }, "Login successful.");
        }
    }
}
