using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentServiceDbContext _context;
        public PaymentRepository(PaymentServiceDbContext context)
            => _context = context;

        public async Task<Payment?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Payments.FirstOrDefaultAsync(
            p => p.Id == id, ct);

        public async Task<Payment?> GetByIdAndUserAsync(
            Guid id, Guid userId, CancellationToken ct = default)
            => await _context.Payments.FirstOrDefaultAsync(
                p => p.Id == id && p.UserId == userId, ct);

        public async Task<List<Payment>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        public async Task<List<Payment>> GetByBillIdAsync(
        Guid billId, Guid userId, CancellationToken ct = default)
        => await _context.Payments
            .Where(p => p.BillId == billId && p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        public async Task AddAsync(Payment payment, CancellationToken ct = default)
        {
            await _context.Payments.AddAsync(payment, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(
            Payment payment, CancellationToken ct = default)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync(ct);
        }
    }
}
