using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.SetDefaultCard
{
    public class SetDefaultCardCommandHandler
    : IRequestHandler<SetDefaultCardCommand, ApiResponse<bool>>
    {
        private readonly ICreditCardRepository _cardRepo;
        public SetDefaultCardCommandHandler(ICreditCardRepository cardRepo)
            => _cardRepo = cardRepo;

        public async Task<ApiResponse<bool>> Handle(
        SetDefaultCardCommand request, CancellationToken ct)
        {
            var card = await _cardRepo.GetByIdAndUserAsync(request.CardId, request.UserId, ct);
            if (card is null || card.IsDeleted)
                return ApiResponse<bool>.FailureResponse("Card not found.");

            // Unset the current default card (if any)
            var currentDefault = await _cardRepo.GetDefaultByUserIdAsync(request.UserId, ct);
            if (currentDefault is not null)
            {
                currentDefault.UnsetDefault();
                await _cardRepo.UpdateAsync(currentDefault, ct);
            }

            // Set the new default
            card.SetAsDefault();
            await _cardRepo.UpdateAsync(card, ct);
            return ApiResponse<bool>.SuccessResponse(true, "Default card updated.");
        }
    }
}
