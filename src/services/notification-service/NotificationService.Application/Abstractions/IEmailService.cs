using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject,
        string htmlBody, CancellationToken ct = default);
    }
}
