using BillingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Interfaces
{
    public interface IRewardRepository
    {
        // RewardTiers
        Task<List<RewardTier>> GetAllTiersAsync(CancellationToken ct = default);
        Task<RewardTier?> GetTierByIdAsync(Guid id, CancellationToken ct = default);
        Task<RewardTier?> GetTierForPointsAsync(int totalPoints, CancellationToken ct = default);

        // RewardAccounts
        Task<RewardAccount?> GetAccountByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task AddAccountAsync(RewardAccount account, CancellationToken ct = default);
        Task UpdateAccountAsync(RewardAccount account, CancellationToken ct = default);

        // RewardTransactions
        Task<List<RewardTransaction>> GetTransactionsByAccountIdAsync(
            Guid accountId, CancellationToken ct = default);
        Task AddTransactionAsync(RewardTransaction transaction, CancellationToken ct = default);
    }
}
