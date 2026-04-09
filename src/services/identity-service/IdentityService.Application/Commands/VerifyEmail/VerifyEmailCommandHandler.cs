using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOTPCodeRepository _otpCodeRepository;
        public VerifyEmailCommandHandler(IUserRepository userRepository, IOTPCodeRepository otpCodeRepository)
        {
            _userRepository = userRepository;
            _otpCodeRepository = otpCodeRepository;
        }

        public async Task<ApiResponse<string>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return ApiResponse<string>.FailureResponse("User not found.");
            if (user.IsEmailVerified)
                return ApiResponse<string>.FailureResponse("Email is already verified.");
            // Find the latest email verification OTP for this user
            var otp = await _otpCodeRepository.GetLatestAsync(user.Id, OTPPurpose.EmailVerification, cancellationToken);
            if (otp is null || !otp.IsValid || otp.Code != request.OTPCode)
                return ApiResponse<string>.FailureResponse("Invalid or expired OTP code.");
            // Mark OTP as used and verify the user
            await _otpCodeRepository.MarkUsedAsync(otp, cancellationToken);
            user.VerifyEmail();
            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<string>.SuccessResponse("Email verified.", "Email verified successfully. You can now login.");
        }
    }
}
