using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetRewardAccount
{
    public record GetRewardAccountQuery(Guid UserId)
    : IRequest<ApiResponse<RewardAccountResponse>>;
}
