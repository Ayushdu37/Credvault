using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.GenerateBill
{
    public record GenerateBillCommand(
    Guid UserId,
    Guid CardId,
    decimal TotalAmount,
    decimal MinimumDue,
    DateTime DueDate,
    string BillingMonth
) : IRequest<ApiResponse<Guid>>;
}
