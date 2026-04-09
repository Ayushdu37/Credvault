using CredVault.Shared.Contracts.Common;
using MediatR;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler
    : IRequestHandler<MarkAsReadCommand, ApiResponse<bool>>
    {
        private readonly INotificationRepository _repo;
        public MarkAsReadCommandHandler(INotificationRepository repo)
            => _repo = repo;

        public async Task<ApiResponse<bool>> Handle(
        MarkAsReadCommand request, CancellationToken ct)
        {
            var notification = await _repo.GetByIdAndUserAsync(
                request.NotificationId, request.UserId, ct);
            if (notification is null)
                return ApiResponse<bool>.FailureResponse(
                    "Notification not found.");

            notification.MarkAsRead();
            await _repo.UpdateAsync(notification, ct);

            return ApiResponse<bool>.SuccessResponse(
                true, "Notification marked as read.");
        }
    }
}
