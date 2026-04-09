using CredVault.Shared.Contracts.Payment.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Messaging.Consumers
{
    public class PaymentFailedConsumer
    : IConsumer<PaymentFailedEvent>
    {
        private readonly INotificationRepository _notifRepo;
        private readonly INotificationPreferenceRepository _prefRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<PaymentFailedConsumer> _logger;
        public PaymentFailedConsumer(
            INotificationRepository notifRepo,
            INotificationPreferenceRepository prefRepo,
            IEmailService emailService,
            ILogger<PaymentFailedConsumer> logger)
        {
            _notifRepo = notifRepo;
            _prefRepo = prefRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var evt = context.Message;

            _logger.LogWarning(
                "Saga compensation: PaymentFailed received. " +
                "PaymentId={PaymentId}, Reason={Reason}",
                evt.PaymentId, evt.Reason);

            // Create in-app notification
            var notification = Notification.Create(
                evt.UserId,
                "PaymentFailed",
                "Payment Failed",
                $"Your payment of ₹{evt.Amount:N2} could not be processed. " +
                $"Reason: {evt.Reason}. Please try again.");

            await _notifRepo.AddAsync(notification);

            // Check preferences and send email
            var prefs = await _prefRepo.GetByUserIdAsync(evt.UserId);

            if (prefs is null || (prefs.EmailEnabled && prefs.PaymentAlerts))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        $"user-{evt.UserId}@credvault.com",
                        "❌ Payment Failed — CredVault",
                        $"<h2>Payment Failed</h2>" +
                        $"<p>Amount: <strong>₹{evt.Amount:N2}</strong></p>" +
                        $"<p>Reason: {evt.Reason}</p>" +
                        $"<p>Please retry your payment.</p>");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to send failure email for PaymentId={PaymentId}",
                        evt.PaymentId);
                }
            }
        }
    }
}
