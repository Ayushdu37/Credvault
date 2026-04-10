using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Events;
using IdentityService.Application.Abstraction;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventPublisher _eventPublisher;

        // Constructor Injection — the DI container provides these automatically
        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _eventPublisher = eventPublisher;
        }

        public async Task<ApiResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                if (existingUser.IsEmailVerified)
                {
                    return ApiResponse<string>.FailureResponse("A user with this email already exists and is verified. Please log in.");
                }
                
                // They are unverified, so we treat it as a "success" so the frontend 
                // redirects them to the verify-email page where it will automatically shoot out a new OTP!
                return ApiResponse<string>.SuccessResponse(existingUser.Id.ToString(), "User exists but is unverified. Redirecting to verification...");
            }

            // 2. Hash the password (never store plain text!)
            var passwordHash = _passwordHasher.Hash(request.Password);

            // 3. Create the user entity
            var user = User.Create(request.Email, passwordHash, request.FullName, request.PhoneNumber);

            // 4. Save to database
            await _userRepository.AddAsync(user, cancellationToken);

            // 5. Publish event to RabbitMQ (Notification Service will send welcome email)
            await _eventPublisher.PublishAsync(new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            }, cancellationToken);

            return ApiResponse<string>.SuccessResponse(user.Id.ToString(), "Registration successful. Please verify your email.");
        }
    }
}
