using PaymentService.Application.Abstractions;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Payment.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.MakePayment
{
    public class MakePaymentCommandHandler
    : IRequestHandler<MakePaymentCommand, ApiResponse<Guid>>
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<MakePaymentCommandHandler> _logger;

        public MakePaymentCommandHandler(
            IPaymentRepository paymentRepo,
            IEventPublisher eventPublisher,
            ILogger<MakePaymentCommandHandler> logger)
        {
            _paymentRepo = paymentRepo;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ApiResponse<Guid>> Handle(
        MakePaymentCommand request, CancellationToken ct)
        {
            // --- SAGA STEP 1: Create payment in Processing state ---
            var payment = Payment.Create(
                request.UserId, request.BillId, request.CardId,
                request.Amount, request.PaymentMethod,
                request.TransactionReference);
            
            await _paymentRepo.AddAsync(payment, ct);
            _logger.LogInformation(
                "Saga started: PaymentId={PaymentId}, Status=Processing",
                payment.Id);

            // --- SAGA STEP 2 (SIMULATED): Immediate success ---
            // In a real app, this would be an async response from a payment gateway
            await SimulatePaymentProcessing(payment, ct);

            return ApiResponse<Guid>.SuccessResponse(
                payment.Id, "Payment processed successfully (Simulated)");
        }

        private async Task SimulatePaymentProcessing(Payment payment, CancellationToken ct)
        {
            _logger.LogInformation("Simulating external payment gateway call for PaymentId={PaymentId}...", payment.Id);
            
            // Simulate 1 second delay
            await Task.Delay(1000, ct);

            // --- SAGA STEP 3: Complete the payment ---
            payment.MarkCompleted();
            await _paymentRepo.UpdateAsync(payment, ct);

            // --- SAGA STEP 4: Publish integration event for other services ---
            var @event = new PaymentCompletedEvent
            {
                PaymentId = payment.Id,
                UserId = payment.UserId,
                BillId = payment.BillId,
                CardId = payment.CardId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Timestamp = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(@event, ct);
            
            _logger.LogInformation(
                "Saga completed: PaymentId={PaymentId}, Status=Completed, EventPublished=true",
                payment.Id);
        }
    }
}
