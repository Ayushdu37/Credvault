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
    /// Consumes UserRegisteredEvent published by Identity Service.
    /// Sends a styled welcome email to the newly registered user.
    /// </summary>
    public class UserRegisteredConsumer
        : IConsumer<UserRegisteredEvent>
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserRegisteredConsumer> _logger;

        public UserRegisteredConsumer(
            INotificationRepository notifRepo,
            IEmailService emailService,
            ILogger<UserRegisteredConsumer> logger)
        {
            _notifRepo = notifRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "UserRegistered received. UserId={UserId}, Email={Email}",
                evt.UserId, evt.Email);

            // Create in-app notification
            var notification = Notification.Create(
                evt.UserId,
                "Welcome",
                "Welcome to CredVault",
                $"Welcome aboard, {evt.FullName}! Your account has been created successfully.");

            await _notifRepo.AddAsync(notification);

            // Send styled welcome email
            try
            {
                await _emailService.SendEmailAsync(
                    evt.Email,
                    "🎉 Welcome to CredVault — Your Account is Ready!",
                    EmailTemplates.WelcomeEmail(evt.FullName));

                _logger.LogInformation(
                    "Welcome email sent to {Email}", evt.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to send welcome email to {Email}", evt.Email);
            }
        }
    }
}
