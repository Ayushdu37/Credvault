using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly BillingServiceDbContext _context;
        public BillRepository(BillingServiceDbContext context)
            => _context = context;

        public async Task<Bill?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Bills.FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task<Bill?> GetByIdAndUserAsync(
            Guid id, Guid userId, CancellationToken ct = default)
            => await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, ct);

        public async Task<List<Bill>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.Bills
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct);

        public async Task<List<Bill>> GetByCardIdAsync(
            Guid cardId, Guid userId, CancellationToken ct = default)
            => await _context.Bills
                .Where(b => b.CardId == cardId && b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(ct);

        public async Task<Bill?> GetByCardAndMonthAsync(
        Guid cardId, string billingMonth, CancellationToken ct = default)
        => await _context.Bills
            .FirstOrDefaultAsync(
                b => b.CardId == cardId && b.BillingMonth == billingMonth, ct);

        public async Task AddAsync(Bill bill, CancellationToken ct = default)
        {
            await _context.Bills.AddAsync(bill, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Bill bill, CancellationToken ct = default)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync(ct);
        }
    }
}