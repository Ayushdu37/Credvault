using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.RemovePaymentMethod
{

    public record RemovePaymentMethodCommand(
        Guid UserId,
        Guid MethodId
    ) : IRequest<ApiResponse<bool>>;
}
