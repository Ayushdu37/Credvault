using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetRewardAccount
{
    public class GetRewardAccountQueryHandler
    : IRequestHandler<GetRewardAccountQuery, ApiResponse<RewardAccountResponse>>
    {
        private readonly IRewardRepository _rewardRepo;
        public GetRewardAccountQueryHandler(IRewardRepository rewardRepo)
            => _rewardRepo = rewardRepo;

        public async Task<ApiResponse<RewardAccountResponse>> Handle(
        GetRewardAccountQuery request, CancellationToken ct)
        {
            var account = await _rewardRepo.GetAccountByUserIdAsync(
            request.UserId, ct);
            if (account is null)
                return ApiResponse<RewardAccountResponse>.FailureResponse(
                    "Reward account not found. Make a payment to create one.");

            // Find the next tier
            var allTiers = await _rewardRepo.GetAllTiersAsync(ct);
            var nextTier = allTiers
                .Where(t => t.MinPoints > account.TotalEarned)
                .OrderBy(t => t.MinPoints)
                .FirstOrDefault();

            return ApiResponse<RewardAccountResponse>.SuccessResponse(
            new RewardAccountResponse
            {
                Id = account.Id,
                TierName = account.Tier.Name,
                CashbackPercent = account.Tier.CashbackPercent,
                AvailablePoints = account.AvailablePoints,
                TotalEarned = account.TotalEarned,
                PointsToNextTier = nextTier is not null
                    ? nextTier.MinPoints - account.TotalEarned : 0,
                NextTierName = nextTier?.Name ?? "Max tier reached"
            });
        }
    }
}
