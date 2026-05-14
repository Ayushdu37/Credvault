using CredVault.Shared.Contracts.Payment.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedConsumer
    : IConsumer<PaymentCompletedEvent>
    {
        private readonly INotificationRepository _notifRepo;
        private readonly INotificationPreferenceRepository _prefRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<PaymentCompletedConsumer> _logger;
        public PaymentCompletedConsumer(
            INotificationRepository notifRepo,
            INotificationPreferenceRepository prefRepo,
            IEmailService emailService,
            ILogger<PaymentCompletedConsumer> logger)
        {
            _notifRepo = notifRepo;
            _prefRepo = prefRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "Saga endpoint: PaymentCompleted received. " +
                "PaymentId={PaymentId}, Amount={Amount}",
                evt.PaymentId, evt.Amount);

            // Create in-app notification
            var notification = Notification.Create(
                evt.UserId,
                "PaymentSuccess",
                "Payment Successful",
                $"Your payment of ₹{evt.Amount:N2} has been processed " +
                $"successfully using {evt.PaymentMethod}. Payment ID: {evt.PaymentId}");

            await _notifRepo.AddAsync(notification);

            // Check preferences and send email
            var prefs = await _prefRepo.GetByUserIdAsync(evt.UserId);
            if (prefs is null || (prefs.EmailEnabled && prefs.PaymentAlerts))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        $"user-{evt.UserId}@credvault.com",
                        "✅ Payment Successful — CredVault",
                        EmailTemplates.PaymentSuccess(evt.Amount, evt.PaymentId));
                    _logger.LogInformation(
                        "Payment success email sent for PaymentId={PaymentId}",
                        evt.PaymentId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to send email for PaymentId={PaymentId}",
                        evt.PaymentId);
                }
            }
        }
    }
}
