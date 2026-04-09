using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.UpdateCardLimit
{
    public class UpdateCardLimitCommandHandler
    : IRequestHandler<UpdateCardLimitCommand, ApiResponse<bool>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public UpdateCardLimitCommandHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<bool>> Handle(
        UpdateCardLimitCommand request, CancellationToken ct)
        {
            var card = await _cardRepo.GetByIdAndUserAsync(request.CardId, request.UserId, ct);
            if (card is null || card.IsDeleted)
                return ApiResponse<bool>.FailureResponse("Card not found.");

            if (request.NewCreditLimit <= 0)
                return ApiResponse<bool>.FailureResponse("Credit limit must be greater than zero.");

            card.UpdateCreditLimit(request.NewCreditLimit);
            await _cardRepo.UpdateAsync(card, ct);

            return ApiResponse<bool>.SuccessResponse(true, "Credit limit updated.");
        }
    }
}
