using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.AddCard
{
    public record AddCardCommand(
    Guid UserId,
    string CardNumber,
    string CardHolderName,
    int ExpiryMonth,
    int ExpiryYear,
    CardIssuer Issuer,
    decimal CreditLimit,
    int BillingCycleStartDay,
    string? Nickname
) : IRequest<ApiResponse<Guid>>;
}
