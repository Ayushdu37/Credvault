using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class NotificationPreferenceRepository
    : INotificationPreferenceRepository
    {
        private readonly NotificationServiceDbContext _context;
        public NotificationPreferenceRepository(
            NotificationServiceDbContext context)
            => _context = context;

        public async Task<NotificationPreference?> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        public async Task AddAsync(NotificationPreference preference,
        CancellationToken ct = default)
        {
            await _context.NotificationPreferences.AddAsync(preference, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(NotificationPreference preference,
            CancellationToken ct = default)
        {
            _context.NotificationPreferences.Update(preference);
            await _context.SaveChangesAsync(ct);
        }
    }
}
