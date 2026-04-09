using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly BillingServiceDbContext _context;
        public RewardRepository(BillingServiceDbContext context)
            => _context = context;

        public async Task<List<RewardTier>> GetAllTiersAsync(
        CancellationToken ct = default)
        => await _context.RewardTiers.OrderBy(t => t.MinPoints).ToListAsync(ct);

        public async Task<RewardTier?> GetTierByIdAsync(
            Guid id, CancellationToken ct = default)
            => await _context.RewardTiers.FindAsync([id], ct);

        public async Task<RewardTier?> GetTierForPointsAsync(
        int totalPoints, CancellationToken ct = default)
        => await _context.RewardTiers
            .Where(t => t.MinPoints <= totalPoints)
            .OrderByDescending(t => t.MinPoints)
            .FirstOrDefaultAsync(ct);

        public async Task<RewardAccount?> GetAccountByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await _context.RewardAccounts
                .Include(a => a.Tier)
                .FirstOrDefaultAsync(a => a.UserId == userId, ct);

        public async Task AddAccountAsync(
        RewardAccount account, CancellationToken ct = default)
        {
            await _context.RewardAccounts.AddAsync(account, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAccountAsync(
            RewardAccount account, CancellationToken ct = default)
        {
            _context.RewardAccounts.Update(account);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<RewardTransaction>> GetTransactionsByAccountIdAsync(
        Guid accountId, CancellationToken ct = default)
        => await _context.RewardTransactions
            .Where(t => t.RewardAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        public async Task AddTransactionAsync(
            RewardTransaction transaction, CancellationToken ct = default)
        {
            await _context.RewardTransactions.AddAsync(transaction, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
