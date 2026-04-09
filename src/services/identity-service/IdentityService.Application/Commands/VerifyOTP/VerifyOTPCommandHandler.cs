using CredVault.Shared.Contracts.Common;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.VerifyOTP
{
    public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOTPCodeRepository _otpCodeRepository;
        public VerifyOTPCommandHandler(IUserRepository userRepository, IOTPCodeRepository otpCodeRepository)
        {
            _userRepository = userRepository;
            _otpCodeRepository = otpCodeRepository;
        }

        public async Task<ApiResponse<string>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return ApiResponse<string>.FailureResponse("User not found.");
            var otp = await _otpCodeRepository.GetLatestAsync(user.Id, request.Purpose, cancellationToken);
            if (otp is null || !otp.IsValid || otp.Code != request.OTPCode)
                return ApiResponse<string>.FailureResponse("Invalid or expired OTP code.");
            await _otpCodeRepository.MarkUsedAsync(otp, cancellationToken);
            return ApiResponse<string>.SuccessResponse("OTP verified.", "OTP verified successfully.");
        }
    }
}
