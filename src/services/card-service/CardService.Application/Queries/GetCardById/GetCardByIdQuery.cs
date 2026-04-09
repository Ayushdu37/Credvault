using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCardById
{
    public record GetCardByIdQuery(Guid UserId, Guid CardId)
    : IRequest<ApiResponse<CardResponse>>;
}
