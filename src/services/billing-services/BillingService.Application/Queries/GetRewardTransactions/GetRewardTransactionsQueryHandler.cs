using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetRewardTransactions
{
    public class GetRewardTransactionsQueryHandler
    : IRequestHandler<GetRewardTransactionsQuery,
        ApiResponse<PaginatedResult<RewardTransactionResponse>>>
    {
        private readonly IRewardRepository _rewardRepo;
        public GetRewardTransactionsQueryHandler(IRewardRepository rewardRepo)
            => _rewardRepo = rewardRepo;

        public async Task<ApiResponse<PaginatedResult<RewardTransactionResponse>>> Handle(
        GetRewardTransactionsQuery request, CancellationToken ct)
        {
            var account = await _rewardRepo.GetAccountByUserIdAsync(
            request.UserId, ct);
            if (account is null)
                return ApiResponse<PaginatedResult<RewardTransactionResponse>>.FailureResponse(
                    "Reward account not found.");

            var transactions = await _rewardRepo
            .GetTransactionsByAccountIdAsync(account.Id, ct);

            var allTransactions = transactions.ToList();
            var totalCount = allTransactions.Count;

            var paged = allTransactions
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new RewardTransactionResponse
                {
                    Id = t.Id,
                    Type = Enum.Parse<RewardTransactionType>(t.Type),
                    Points = t.Points,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                }).ToList();

            var result = PaginatedResult<RewardTransactionResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<RewardTransactionResponse>>
            .SuccessResponse(result);
        }
    }
}
