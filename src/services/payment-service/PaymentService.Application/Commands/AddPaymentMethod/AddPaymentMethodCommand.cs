using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.AddPaymentMethod
{
    public record AddPaymentMethodCommand(
    Guid UserId,
    string MethodType,
    string DisplayName,
    string Details
) : IRequest<ApiResponse<Guid>>;
}
