using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationPreferenceRepository
    {
        Task<NotificationPreference?> GetByUserIdAsync(Guid userId,
        CancellationToken ct = default);
        Task AddAsync(NotificationPreference preference,
            CancellationToken ct = default);
        Task UpdateAsync(NotificationPreference preference,
            CancellationToken ct = default);
    }
}
