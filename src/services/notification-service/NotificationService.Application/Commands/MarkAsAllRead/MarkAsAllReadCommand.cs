using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.MarkAsAllRead
{
    public record MarkAllAsReadCommand(Guid UserId)
    : IRequest<ApiResponse<bool>>;
}
