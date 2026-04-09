using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetUnreadCount
{
    public class GetUnreadCountQueryHandler
    : IRequestHandler<GetUnreadCountQuery,
        ApiResponse<UnreadCountResponse>>
    {
        private readonly INotificationRepository _repo;
        public GetUnreadCountQueryHandler(INotificationRepository repo)
            => _repo = repo;

        public async Task<ApiResponse<UnreadCountResponse>> Handle(
            GetUnreadCountQuery request, CancellationToken ct)
        {
            var count = await _repo.GetUnreadCountAsync(
                request.UserId, ct);

            return ApiResponse<UnreadCountResponse>.SuccessResponse(
                new UnreadCountResponse { Count = count });
        }
    }
}
