using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.RemoveCard
{
    public class RemoveCardCommandHandler
    : IRequestHandler<RemoveCardCommand, ApiResponse<bool>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public RemoveCardCommandHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<bool>> Handle(
        RemoveCardCommand request, CancellationToken ct)
        {
            var card = await _cardRepo.GetByIdAndUserAsync(request.CardId, request.UserId, ct);
            if (card is null)
                return ApiResponse<bool>.FailureResponse("Card not found.");

            if (card.IsDeleted)
                return ApiResponse<bool>.FailureResponse("Card has already been removed.");

            card.SoftDelete();
            await _cardRepo.UpdateAsync(card, ct);

            return ApiResponse<bool>.SuccessResponse(true, "Card removed successfully.");
        }
    }
}
