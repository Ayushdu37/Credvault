using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    public class PaymentScheduleRepository : IPaymentScheduleRepository
    {
        private readonly BillingServiceDbContext _context;
        public PaymentScheduleRepository(BillingServiceDbContext context)
            => _context = context;

        public async Task<PaymentSchedule?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.PaymentSchedules
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<PaymentSchedule?> GetByIdAndUserAsync(
            Guid id, Guid userId, CancellationToken ct = default)
            => await _context.PaymentSchedules
                .FirstOrDefaultAsync(
                    p => p.Id == id && p.UserId == userId, ct);

        public async Task<List<PaymentSchedule>> GetByBillIdAsync(
        Guid billId, CancellationToken ct = default)
        => await _context.PaymentSchedules
            .Where(p => p.BillId == billId)
            .OrderBy(p => p.ScheduledDate)
            .ToListAsync(ct);

        public async Task<List<PaymentSchedule>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await _context.PaymentSchedules
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.ScheduledDate)
                .ToListAsync(ct);

        public async Task<List<PaymentSchedule>> GetPendingByDateAsync(
        DateTime date, CancellationToken ct = default)
        => await _context.PaymentSchedules
            .Where(p => p.Status == "Pending" && p.ScheduledDate <= date)
            .ToListAsync(ct);

        public async Task AddAsync(
        PaymentSchedule schedule, CancellationToken ct = default)
        {
            await _context.PaymentSchedules.AddAsync(schedule, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(
            PaymentSchedule schedule, CancellationToken ct = default)
        {
            _context.PaymentSchedules.Update(schedule);
            await _context.SaveChangesAsync(ct);
        }
    }
}
