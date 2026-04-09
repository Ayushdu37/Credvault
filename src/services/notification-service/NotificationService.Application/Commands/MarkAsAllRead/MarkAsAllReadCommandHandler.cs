using CredVault.Shared.Contracts.Common;
using MediatR;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.MarkAsAllRead
{
    public class MarkAllAsReadCommandHandler
    : IRequestHandler<MarkAllAsReadCommand, ApiResponse<bool>>
    {
        private readonly INotificationRepository _repo;

        public MarkAllAsReadCommandHandler(INotificationRepository repo)
            => _repo = repo;

        public async Task<ApiResponse<bool>> Handle(
            MarkAllAsReadCommand request, CancellationToken ct)
        {
            await _repo.MarkAllAsReadAsync(request.UserId, ct);
            return ApiResponse<bool>.SuccessResponse(
                true, "All notifications marked as read.");
        }
    }
}
