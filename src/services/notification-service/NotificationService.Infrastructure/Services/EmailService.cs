using NotificationService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

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

            var senderName = smtpSettings["SenderName"] ?? "CredVault";
            var senderEmail = smtpSettings["SenderEmail"] ?? "noreply@credvault.com";
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var username = smtpSettings["Username"] ?? "";
            var password = smtpSettings["Password"] ?? "";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message, ct);
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
