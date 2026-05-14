using CardService.Domain.Interfaces;
using CredVault.Shared.Contracts.Payment.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CardService.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly ICreditCardRepository _cardRepo;
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        // Only CreditCard charges should increase the outstanding balance
        private static readonly HashSet<string> CardBasedMethods = new(StringComparer.OrdinalIgnoreCase)
        {
            "CreditCard",
            "4"  // CreditCard Enum integer representation
        };

        public PaymentCompletedConsumer(
            ICreditCardRepository cardRepo,
            ILogger<PaymentCompletedConsumer> logger)
        {
            _cardRepo = cardRepo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation(
                "CardService received PaymentCompleted: PaymentId={PaymentId}, CardId={CardId}, Amount={Amount}, Method={Method}",
                msg.PaymentId, msg.CardId, msg.Amount, msg.PaymentMethod);

            var card = await _cardRepo.GetByIdAsync(msg.CardId);
            if (card is null)
            {
                _logger.LogWarning(
                    "Card {CardId} not found — skipping balance update for PaymentId={PaymentId}",
                    msg.CardId, msg.PaymentId);
                return;
            }

            // Determine direction based on PaymentMethod.
            // If PaymentMethod is CreditCard, this card was used as the SOURCE -> Debt INCREASES.
            // Otherwise, we assume this card is the DESTINATION (being paid off) -> Debt DECREASES.
            bool isSource = CardBasedMethods.Contains(msg.PaymentMethod);

            if (isSource)
            {
                card.UpdateOutstandingBalance(msg.Amount);
            }
            else
            {
                card.UpdateOutstandingBalance(-msg.Amount);
            }

            await _cardRepo.UpdateAsync(card);

            _logger.LogInformation(
                "Card {CardId} balance updated (isSource={IsSource}): NewOutstanding={Outstanding}, NewAvailable={Available}",
                card.Id, isSource, card.OutstandingBalance, card.AvailableCredit);
        }
    }
}
