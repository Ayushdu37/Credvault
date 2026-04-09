using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetRewardTransactions
{
    public record GetRewardTransactionsQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<ApiResponse<PaginatedResult<RewardTransactionResponse>>>;
}
