using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Repositories
{
    public class SavedPaymentMethodRepository : ISavedPaymentMethodRepository
    {
        private readonly PaymentServiceDbContext _context;
        public SavedPaymentMethodRepository(PaymentServiceDbContext context)
            => _context = context;

        public async Task<SavedPaymentMethod?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.SavedPaymentMethods
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task<SavedPaymentMethod?> GetByIdAndUserAsync(
            Guid id, Guid userId, CancellationToken ct = default)
            => await _context.SavedPaymentMethods
                .FirstOrDefaultAsync(
                    m => m.Id == id && m.UserId == userId, ct);

        public async Task<List<SavedPaymentMethod>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.SavedPaymentMethods
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

        public async Task<SavedPaymentMethod?> GetDefaultByUserAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.SavedPaymentMethods
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.IsDefault, ct);

        public async Task AddAsync(
        SavedPaymentMethod method, CancellationToken ct = default)
        {
            await _context.SavedPaymentMethods.AddAsync(method, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(
            SavedPaymentMethod method, CancellationToken ct = default)
        {
            _context.SavedPaymentMethods.Update(method);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(
            SavedPaymentMethod method, CancellationToken ct = default)
        {
            _context.SavedPaymentMethods.Remove(method);
            await _context.SaveChangesAsync(ct);
        }
    }
}
