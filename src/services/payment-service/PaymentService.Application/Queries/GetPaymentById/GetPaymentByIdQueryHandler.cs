
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using CredVault.Shared.Contracts.Payment.Responses;
using MediatR;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler
    : IRequestHandler<GetPaymentByIdQuery, ApiResponse<PaymentResponse>>
    {
        private readonly IPaymentRepository _paymentRepo;
        public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepo)
            => _paymentRepo = paymentRepo;

        public async Task<ApiResponse<PaymentResponse>> Handle(
        GetPaymentByIdQuery request, CancellationToken ct)
        {
            var p = await _paymentRepo.GetByIdAndUserAsync(
            request.PaymentId, request.UserId, ct);
            if (p is null)
                return ApiResponse<PaymentResponse>.FailureResponse(
                    "Payment not found.");

            return ApiResponse<PaymentResponse>.SuccessResponse(
            new PaymentResponse
            {
                Id = p.Id,
                BillId = p.BillId,
                CardId = p.CardId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionReference = p.TransactionReference,
                Status = Enum.Parse<PaymentStatus>(p.Status),
                CreatedAt = p.CreatedAt
            });
        }
    }
}
