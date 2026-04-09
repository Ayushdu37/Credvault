using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Card.Responses;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Queries.GetCardUtilization
{
    public class GetCardUtilizationQueryHandler
    : IRequestHandler<GetCardUtilizationQuery, ApiResponse<CardSummaryResponse>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public GetCardUtilizationQueryHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<CardSummaryResponse>> Handle(
        GetCardUtilizationQuery request, CancellationToken ct)
        {
            var cards = await _cardRepo.GetByUserIdAsync(request.UserId, ct);
            var activeCards = cards.Where(c => !c.IsDeleted).ToList();

            var totalLimit = activeCards.Sum(c => c.CreditLimit);
            var totalBalance = activeCards.Sum(c => c.OutstandingBalance);

            var response = new CardSummaryResponse
            {
                TotalCards = activeCards.Count,
                TotalCreditLimit = totalLimit,
                TotalOutstandingBalance = totalBalance,
                TotalAvailableCredit = totalLimit - totalBalance,
                UtilizationPercentage = totalLimit > 0
                    ? Math.Round(totalBalance / totalLimit * 100, 2)
                    : 0
            };

            return ApiResponse<CardSummaryResponse>.SuccessResponse(response);
        }
    }
}
