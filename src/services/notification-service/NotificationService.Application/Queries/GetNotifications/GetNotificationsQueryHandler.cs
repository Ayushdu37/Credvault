using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery,
        ApiResponse<PaginatedResult<NotificationResponse>>>
    {
        private readonly INotificationRepository _repo;
        public GetNotificationsQueryHandler(INotificationRepository repo)
            => _repo = repo;

        public async Task<ApiResponse<PaginatedResult<NotificationResponse>>> Handle(
        GetNotificationsQuery request, CancellationToken ct)
        {
            var notifications = await _repo.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize, ct);

            var allNotifications = notifications.ToList();

            // Get total count from the full (unpaginated) set
            // Note: The repo already paginates, so we need total count separately
            var totalNotifications = await _repo.GetByUserIdAsync(
                request.UserId, 1, int.MaxValue, ct);
            var totalCount = totalNotifications.Count();

            var paged = allNotifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            var result = PaginatedResult<NotificationResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<NotificationResponse>>
            .SuccessResponse(result);
        }
    }
}
