using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCards
{
    public class GetCardsQueryHandler
    : IRequestHandler<GetCardsQuery, ApiResponse<PaginatedResult<CardResponse>>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public GetCardsQueryHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<PaginatedResult<CardResponse>>> Handle(
        GetCardsQuery request, CancellationToken ct)
        {
            var cards = await _cardRepo.GetByUserIdAsync(request.UserId, ct);
            var filtered = cards.Where(c => !c.IsDeleted).ToList();

            var totalCount = filtered.Count;
            var paged = filtered
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CardResponse
                {
                    Id = c.Id,
                    MaskedNumber = c.MaskedNumber,
                    CardHolderName = c.CardHolderName,
                    Issuer = (CredVault.Shared.Contracts.Enums.CardIssuer)
                        Enum.Parse(typeof(CredVault.Shared.Contracts.Enums.CardIssuer),
                        c.Issuer.Name),
                    IssuerName = c.Issuer.Name,
                    Nickname = c.MaskedNumber,
                    ExpiryMonth = c.ExpiryMonth,
                    ExpiryYear = c.ExpiryYear,
                    CreditLimit = c.CreditLimit,
                    OutstandingBalance = c.OutstandingBalance,
                    AvailableCredit = c.AvailableCredit,
                    BillingCycleStartDay = c.BillingCycleStartDay,
                    IsDefault = c.IsDefault,
                    IsVerified = c.IsVerified,
                    AddedAt = c.CreatedAt
                })
                .ToList();

            var result = PaginatedResult<CardResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<CardResponse>>.SuccessResponse(result);
        }
    }
}
