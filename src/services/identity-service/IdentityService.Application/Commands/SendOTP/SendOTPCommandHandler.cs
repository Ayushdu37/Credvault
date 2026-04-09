using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Events;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.SendOTP
{
    public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOTPCodeRepository _otpCodeRepository;
        private readonly IEventPublisher _eventPublisher;
        public SendOTPCommandHandler(
            IUserRepository userRepository,
            IOTPCodeRepository otpCodeRepository,
            IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _otpCodeRepository = otpCodeRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<ApiResponse<string>> Handle(SendOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return ApiResponse<string>.FailureResponse("User not found.");
            // Generate a 6-digit OTP
            var code = Random.Shared.Next(100000, 999999).ToString();
            var otp = OTPCode.Create(user.Id, code, request.Purpose);
            await _otpCodeRepository.AddAsync(otp, cancellationToken);
            // Publish event — Notification Service will email/SMS this code
            await _eventPublisher.PublishAsync(new OTPRequestedEvent
            {
                UserId = user.Id,
                Email = user.Email,
                OTPCode = code,
                Purpose = request.Purpose
            }, cancellationToken);
            return ApiResponse<string>.SuccessResponse("OTP sent.", "OTP has been sent to your email.");
        }
    }
}
