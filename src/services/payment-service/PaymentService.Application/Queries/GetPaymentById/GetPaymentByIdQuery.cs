using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Payment.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Queries.GetPaymentById
{
    public record GetPaymentByIdQuery(Guid UserId, Guid PaymentId)
    : IRequest<ApiResponse<PaymentResponse>>;
}
