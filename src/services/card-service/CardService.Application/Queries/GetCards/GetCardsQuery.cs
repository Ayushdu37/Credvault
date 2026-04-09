using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCards
{
    public record GetCardsQuery(Guid UserId, int Page = 1, int PageSize = 10)
    : IRequest<ApiResponse<PaginatedResult<CardResponse>>>;
}
