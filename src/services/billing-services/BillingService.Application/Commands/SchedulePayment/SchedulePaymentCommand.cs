using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.SchedulePayment
{
    public record SchedulePaymentCommand(
    Guid UserId,
    Guid BillId,
    decimal Amount,
    DateTime ScheduledDate
) : IRequest<ApiResponse<Guid>>;
}
