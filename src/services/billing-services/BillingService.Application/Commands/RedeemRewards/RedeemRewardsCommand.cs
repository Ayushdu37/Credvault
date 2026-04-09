using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.RedeemRewards
{
    public record RedeemRewardsCommand(
    Guid UserId,
    int PointsToRedeem
) : IRequest<ApiResponse<bool>>;
}
