using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetBillById
{
    public record GetBillByIdQuery(Guid UserId, Guid BillId)
    : IRequest<ApiResponse<BillResponse>>;
}
