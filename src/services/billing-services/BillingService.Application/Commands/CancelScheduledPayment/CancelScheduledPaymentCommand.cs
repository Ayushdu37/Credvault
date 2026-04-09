using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.CancelScheduledPayment
{
    public record CancelScheduledPaymentCommand(
    Guid UserId,
    Guid ScheduleId
) : IRequest<ApiResponse<bool>>;
}