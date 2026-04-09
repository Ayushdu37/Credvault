using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOTPCodeRepository _otpCodeRepository;
        private readonly IPasswordHasher _passwordHasher;
        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IOTPCodeRepository otpCodeRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _otpCodeRepository = otpCodeRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return ApiResponse<string>.FailureResponse("User not found.");

            // Verify the OTP is valid for password reset
            var otp = await _otpCodeRepository.GetLatestAsync(user.Id, OTPPurpose.PasswordReset, cancellationToken);
            if (otp is null || !otp.IsValid || otp.Code != request.OTPCode)
                return ApiResponse<string>.FailureResponse("Invalid or expired OTP code.");

            // Mark OTP as used
            await _otpCodeRepository.MarkUsedAsync(otp, cancellationToken);

            // Hash the new password and update
            var newHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatePassword(newHash);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<string>.SuccessResponse("Password reset.", "Password has been reset successfully.");
        }
    }
}
