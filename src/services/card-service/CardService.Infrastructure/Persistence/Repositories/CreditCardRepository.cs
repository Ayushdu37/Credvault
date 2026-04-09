using CardService.Domain.Entities;
using CardService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure.Persistence.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly CardServiceDbContext _context;
        public CreditCardRepository(CardServiceDbContext context)
            => _context = context;

        public async Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CreditCards
            .Include(c => c.Issuer)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<CreditCard?> GetByIdAndUserAsync(
            Guid id, Guid userId, CancellationToken ct = default)
            => await _context.CreditCards
                .Include(c => c.Issuer)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        public async Task<List<CreditCard>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.CreditCards
            .Include(c => c.Issuer)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IsDefault)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        public async Task<CreditCard?> GetDefaultByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.CreditCards
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDefault, ct);

        public async Task<bool> ExistsByHashAsync(
            Guid userId, string cardNumberHash, CancellationToken ct = default)
            => await _context.CreditCards
                .IgnoreQueryFilters()  // Check even deleted cards!
                .AnyAsync(c => c.UserId == userId
                    && c.CardNumberHash == cardNumberHash, ct);

        public async Task AddAsync(CreditCard card, CancellationToken ct = default)
        {
            await _context.CreditCards.AddAsync(card, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(CreditCard card, CancellationToken ct = default)
        {
            _context.CreditCards.Update(card);
            await _context.SaveChangesAsync(ct);
        }
    }
}
