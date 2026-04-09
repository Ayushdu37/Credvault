using NotificationService.Application.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration config,
            ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject,
        string htmlBody, CancellationToken ct = default)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                smtpSettings["SenderName"] ?? "CredVault",
                smtpSettings["SenderEmail"] ?? "noreply@credvault.com"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    smtpSettings["Host"] ?? "smtp.ethereal.email",
                    int.Parse(smtpSettings["Port"] ?? "587"),
                    MailKit.Security.SecureSocketOptions.StartTls, ct);
                await client.AuthenticateAsync(
                    smtpSettings["Username"] ?? "",
                    smtpSettings["Password"] ?? "", ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
                _logger.LogInformation(
                    "Email sent to {ToEmail}: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Email failed to {ToEmail}: {Subject}", toEmail, subject);
                // Don't throw — email failure shouldn't break the flow
            }
        }
    }
}
