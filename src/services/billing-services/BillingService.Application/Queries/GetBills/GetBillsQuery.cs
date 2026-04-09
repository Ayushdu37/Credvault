using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetBills
{
    public record GetBillsQuery(Guid UserId, int Page = 1, int PageSize = 10)
    : IRequest<ApiResponse<PaginatedResult<BillResponse>>>;
}
