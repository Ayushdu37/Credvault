using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCardById
{
    public class GetCardByIdQueryHandler
    : IRequestHandler<GetCardByIdQuery, ApiResponse<CardResponse>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public GetCardByIdQueryHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<CardResponse>> Handle(
        GetCardByIdQuery request, CancellationToken ct)
        {
            var card = await _cardRepo.GetByIdAndUserAsync(request.CardId, request.UserId, ct);
            if (card is null || card.IsDeleted)
                return ApiResponse<CardResponse>.FailureResponse("Card not found.");

            var response = new CardResponse
            {
                Id = card.Id,
                MaskedNumber = card.MaskedNumber,
                CardHolderName = card.CardHolderName,
                Issuer = (CredVault.Shared.Contracts.Enums.CardIssuer)
                Enum.Parse(typeof(CredVault.Shared.Contracts.Enums.CardIssuer),
                card.Issuer.Name),
                IssuerName = card.Issuer.Name,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                CreditLimit = card.CreditLimit,
                OutstandingBalance = card.OutstandingBalance,
                AvailableCredit = card.AvailableCredit,
                BillingCycleStartDay = card.BillingCycleStartDay,
                IsDefault = card.IsDefault,
                IsVerified = card.IsVerified,
                AddedAt = card.CreatedAt
            };

            return ApiResponse<CardResponse>.SuccessResponse(response);
        }
    }
}
