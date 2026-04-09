using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.VerifyCard
{
    public class VerifyCardCommandHandler
    : IRequestHandler<VerifyCardCommand, ApiResponse<bool>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public VerifyCardCommandHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<bool>> Handle(
        VerifyCardCommand request, CancellationToken ct)
        {
            var card = await _cardRepo.GetByIdAndUserAsync(request.CardId, request.UserId, ct);
            if (card is null || card.IsDeleted)
                return ApiResponse<bool>.FailureResponse("Card not found.");

            if (card.IsVerified)
                return ApiResponse<bool>.FailureResponse("Card is already verified.");

            card.Verify();
            await _cardRepo.UpdateAsync(card, ct);

            return ApiResponse<bool>.SuccessResponse(true, "Card verified successfully.");
        }
    }
}
