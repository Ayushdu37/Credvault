using CardService.Application.Abstractions;
using CardService.Domain.Entities;
using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Card.Events;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Commands.AddCard
{
    public class AddCardCommandHandler
    : IRequestHandler<AddCardCommand, ApiResponse<Guid>>
    {
        private readonly ICreditCardRepository _cardRepo;
        private readonly ICardIssuerRepository _issuerRepo;
        private readonly ICardHasher _hasher;
        private readonly IEventPublisher _events;
        public AddCardCommandHandler(
            ICreditCardRepository cardRepo,
            ICardIssuerRepository issuerRepo,
            ICardHasher hasher,
            IEventPublisher events)
        {
            _cardRepo = cardRepo;
            _issuerRepo = issuerRepo;
            _hasher = hasher;
            _events = events;
        }

        public async Task<ApiResponse<Guid>> Handle(
        AddCardCommand request, CancellationToken ct)
        {
            // 1. Hash the card number for duplicate check
            var cardHash = _hasher.HashCardNumber(request.CardNumber);

            // 2. Check if this card already exists for this user
            if (await _cardRepo.ExistsByHashAsync(request.UserId, cardHash, ct))
                return ApiResponse<Guid>.FailureResponse("This card has already been added.");

            // 3. Find the issuer
            var issuer = await _issuerRepo.GetByNameAsync(request.Issuer.ToString(), ct);
            if (issuer is null)
                return ApiResponse<Guid>.FailureResponse("Unknown card issuer.");

            // 4. Create the masked number (e.g., "**** **** **** 1234")
            var maskedNumber = _hasher.MaskCardNumber(request.CardNumber);

            // 5. Create the entity
            var card = CreditCard.Create(
                userId: request.UserId,
                maskedNumber: maskedNumber,
                cardNumberHash: cardHash,
                cardHolderName: request.CardHolderName,
                expiryMonth: request.ExpiryMonth,
                expiryYear: request.ExpiryYear,
                issuerId: issuer.Id,
                creditLimit: request.CreditLimit,
                billingCycleStartDay: request.BillingCycleStartDay);
            await _cardRepo.AddAsync(card, ct);

            // 6. Publish event
            await _events.PublishAsync(new CardAddedEvent
            {
                CardId = card.Id,
                UserId = card.UserId,
                CardNickname = request.Nickname ?? maskedNumber,
                IssuerName = issuer.Name,
                Last4Digits = maskedNumber[^4..]
            }, ct);

            return ApiResponse<Guid>.SuccessResponse(card.Id, "Card added successfully.");
        }
    }
}
