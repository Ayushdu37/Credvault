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
        private readonly IEventPublisher _events;
        private readonly ILogger<MakePaymentCommandHandler> _logger;
        public MakePaymentCommandHandler(
            IPaymentRepository paymentRepo,
            IEventPublisher events,
            ILogger<MakePaymentCommandHandler> logger)
        {
            _paymentRepo = paymentRepo;
            _events = events;
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

            try
            {
                // --- SAGA STEP 2: Simulate payment processing ---
                // In production: call Stripe/Razorpay API here
                await SimulatePaymentProcessing(payment, ct);

                // --- SAGA STEP 3: Success → mark Completed ---
                payment.MarkCompleted();
                await _paymentRepo.UpdateAsync(payment, ct);

                _logger.LogInformation(
                    "Saga success: PaymentId={PaymentId}, Status=Completed",
                    payment.Id);
                // Publish PaymentCompletedEvent
                // → Billing Service consumes → updates bill + earns rewards
                await _events.PublishAsync(new PaymentCompletedEvent
                {
                    PaymentId = payment.Id,
                    UserId = payment.UserId,
                    CardId = payment.CardId,
                    BillId = payment.BillId,
                    Amount = payment.Amount
                }, ct);

                return ApiResponse<Guid>.SuccessResponse(
                    payment.Id, "Payment completed successfully.");
            }
            catch (Exception ex)
            {
                // --- SAGA COMPENSATION: Mark as Failed ---
                payment.MarkFailed(ex.Message);
                await _paymentRepo.UpdateAsync(payment, ct);
                _logger.LogWarning(ex,
                    "Saga compensation: PaymentId={PaymentId}, Status=Failed",
                    payment.Id);
                // Publish PaymentFailedEvent → other services can react
                await _events.PublishAsync(new PaymentFailedEvent
                {
                    PaymentId = payment.Id,
                    UserId = payment.UserId,
                    BillId = payment.BillId,
                    Amount = payment.Amount,
                    Reason = ex.Message
                }, ct);
                return ApiResponse<Guid>.FailureResponse(
                    $"Payment failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Simulates external payment gateway processing.
        /// In production, replace with actual Stripe/Razorpay call.
        /// </summary>
        private static async Task SimulatePaymentProcessing(
            Payment payment, CancellationToken ct)
        {
            // Simulate API latency
            await Task.Delay(500, ct);
            // Simulate failure for amounts exactly ₹99,999 (for testing)
            if (payment.Amount == 99999)
                throw new InvalidOperationException(
                    "Payment gateway declined the transaction.");
        }
    }
}
