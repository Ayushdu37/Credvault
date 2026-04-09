using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Payment.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Queries.GetPayments
{
    public record GetPaymentsQuery(Guid UserId, int Page = 1, int PageSize = 10)
    : IRequest<ApiResponse<PaginatedResult<PaymentResponse>>>;
}
