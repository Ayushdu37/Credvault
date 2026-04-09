using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetUnreadCount
{
    public record GetUnreadCountQuery(Guid UserId)
    : IRequest<ApiResponse<UnreadCountResponse>>;
}
