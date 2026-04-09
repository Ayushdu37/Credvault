using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.MakePayment
{
    public record MakePaymentCommand(
    Guid UserId,
    Guid BillId,
    Guid CardId,
    decimal Amount,
    string PaymentMethod,
    string? TransactionReference
) : IRequest<ApiResponse<Guid>>;
}
