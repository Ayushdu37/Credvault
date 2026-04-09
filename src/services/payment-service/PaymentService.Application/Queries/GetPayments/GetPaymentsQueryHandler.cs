using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using CredVault.Shared.Contracts.Payment.Responses;
using MediatR;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Queries.GetPayments
{
    public class GetPaymentsQueryHandler
    : IRequestHandler<GetPaymentsQuery, ApiResponse<PaginatedResult<PaymentResponse>>>
    {
        private readonly IPaymentRepository _paymentRepo;
        public GetPaymentsQueryHandler(IPaymentRepository paymentRepo)
            => _paymentRepo = paymentRepo;

        public async Task<ApiResponse<PaginatedResult<PaymentResponse>>> Handle(
        GetPaymentsQuery request, CancellationToken ct)
        {
            var payments = await _paymentRepo.GetByUserIdAsync(
                request.UserId, ct);

            var allPayments = payments.ToList();
            var totalCount = allPayments.Count;

            var paged = allPayments
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    BillId = p.BillId,
                    CardId = p.CardId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    Status = Enum.Parse<PaymentStatus>(p.Status),
                    CreatedAt = p.CreatedAt
                }).ToList();

            var result = PaginatedResult<PaymentResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<PaymentResponse>>.SuccessResponse(result);
        }
    }
}
