using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.RedeemRewards
{
    public class RedeemRewardsCommandHandler
    : IRequestHandler<RedeemRewardsCommand, ApiResponse<bool>>
    {
        private readonly IRewardRepository _rewardRepo;
        public RedeemRewardsCommandHandler(IRewardRepository rewardRepo)
            => _rewardRepo = rewardRepo;

        public async Task<ApiResponse<bool>> Handle(
        RedeemRewardsCommand request, CancellationToken ct)
        {
            var account = await _rewardRepo.GetAccountByUserIdAsync(
                request.UserId, ct);
            if (account is null)
                return ApiResponse<bool>.FailureResponse("Reward account not found.");

            if (!account.CanRedeem(request.PointsToRedeem))
                return ApiResponse<bool>.FailureResponse(
                    $"Insufficient points. Available: {account.AvailablePoints}");

            account.RedeemPoints(request.PointsToRedeem);
            await _rewardRepo.UpdateAccountAsync(account, ct);

            var transaction = RewardTransaction.CreateRedeemed(
                account.Id, request.PointsToRedeem,
                $"Redeemed {request.PointsToRedeem} points");
            await _rewardRepo.AddTransactionAsync(transaction, ct);

            return ApiResponse<bool>.SuccessResponse(true,
                $"Redeemed {request.PointsToRedeem} points.");
        }
    }
}
