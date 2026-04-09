using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCardUtilization
{
    public record GetCardUtilizationQuery(Guid UserId)
    : IRequest<ApiResponse<CardSummaryResponse>>;
}
