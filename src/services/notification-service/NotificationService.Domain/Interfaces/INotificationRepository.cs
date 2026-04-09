using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAndUserAsync(Guid id, Guid userId,
        CancellationToken ct = default);
        Task<List<Notification>> GetByUserIdAsync(Guid userId,
            int page = 1, int pageSize = 20,
            CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(Guid userId,
            CancellationToken ct = default);
        Task AddAsync(Notification notification,
            CancellationToken ct = default);
        Task UpdateAsync(Notification notification,
            CancellationToken ct = default);
        Task MarkAllAsReadAsync(Guid userId,
            CancellationToken ct = default);
    }
}
