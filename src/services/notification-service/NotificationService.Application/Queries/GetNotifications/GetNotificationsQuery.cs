using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetNotifications
{
    public record GetNotificationsQuery(
    Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedResult<NotificationResponse>>>;
}
