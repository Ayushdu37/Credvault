using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationServiceDbContext _context;
        public NotificationRepository(NotificationServiceDbContext context)
            => _context = context;

        public async Task<Notification?> GetByIdAndUserAsync(
        Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Notifications.FirstOrDefaultAsync(
            n => n.Id == id && n.UserId == userId, ct);

        public async Task<List<Notification>> GetByUserIdAsync(
        Guid userId, int page = 1, int pageSize = 20,
        CancellationToken ct = default)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        public async Task<int> GetUnreadCountAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);

        public async Task AddAsync(Notification notification,
        CancellationToken ct = default)
        {
            await _context.Notifications.AddAsync(notification, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Notification notification,
            CancellationToken ct = default)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync(ct);
        }

        public async Task MarkAllAsReadAsync(Guid userId,
        CancellationToken ct = default)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(n => n.IsRead, true)
                     .SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);
        }
    }
}
