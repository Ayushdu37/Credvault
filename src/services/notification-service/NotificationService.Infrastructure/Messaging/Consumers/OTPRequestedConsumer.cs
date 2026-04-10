using CredVault.Shared.Contracts.Identity.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.Messaging.Consumers
{
    /// <summary>
    /// Consumes OTPRequestedEvent published by Identity Service.
    /// Sends a styled OTP email to the user for email verification, password reset, or login.
    /// </summary>
    public class OTPRequestedConsumer
        : IConsumer<OTPRequestedEvent>
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<OTPRequestedConsumer> _logger;

        public OTPRequestedConsumer(
            INotificationRepository notifRepo,
            IEmailService emailService,
            ILogger<OTPRequestedConsumer> logger)
        {
            _notifRepo = notifRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OTPRequestedEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "OTPRequested received. UserId={UserId}, Purpose={Purpose}",
                evt.UserId, evt.Purpose);

            // Determine subject based on purpose
            var (subject, purposeLabel) = evt.Purpose switch
            {
                CredVault.Shared.Contracts.Enums.OTPPurpose.EmailVerification
                    => ("🔐 Verify Your Email — CredVault", "Email Verification"),
                CredVault.Shared.Contracts.Enums.OTPPurpose.PasswordReset
                    => ("🔑 Password Reset — CredVault", "Password Reset"),
                CredVault.Shared.Contracts.Enums.OTPPurpose.Login
                    => ("🔓 Login Verification — CredVault", "Login Verification"),
                _ => ("🔐 Your OTP Code — CredVault", "Verification")
            };

            // Create in-app notification
            var notification = Notification.Create(
                evt.UserId,
                "OTPRequested",
                purposeLabel,
                $"Your OTP code for {purposeLabel.ToLower()} is: {evt.OTPCode}");

            await _notifRepo.AddAsync(notification);

            // Send styled email
            try
            {
                await _emailService.SendEmailAsync(
                    evt.Email,
                    subject,
                    EmailTemplates.OTPEmail(evt.OTPCode, purposeLabel));

                _logger.LogInformation(
                    "OTP email sent to {Email} for {Purpose}",
                    evt.Email, evt.Purpose);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to send OTP email to {Email}", evt.Email);
            }
        }
    }
}
