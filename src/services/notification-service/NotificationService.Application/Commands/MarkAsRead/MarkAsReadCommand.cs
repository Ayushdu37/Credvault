using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid UserId, Guid NotificationId)
    : IRequest<ApiResponse<bool>>;
}
