using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetBillById
{
    public class GetBillByIdQueryHandler
    : IRequestHandler<GetBillByIdQuery, ApiResponse<BillResponse>>
    {
        private readonly IBillRepository _billRepo;
        public GetBillByIdQueryHandler(IBillRepository billRepo)
            => _billRepo = billRepo;

        public async Task<ApiResponse<BillResponse>> Handle(
        GetBillByIdQuery request, CancellationToken ct)
        {
            var bill = await _billRepo.GetByIdAndUserAsync(
                request.BillId, request.UserId, ct);
            if (bill is null || bill.IsDeleted)
                return ApiResponse<BillResponse>.FailureResponse("Bill not found.");

            return ApiResponse<BillResponse>.SuccessResponse(new BillResponse
            {
                Id = bill.Id,
                CardId = bill.CardId,
                TotalAmount = bill.TotalAmount,
                MinimumDue = bill.MinimumDue,
                AmountPaid = bill.AmountPaid,
                DueDate = bill.DueDate,
                BillingMonth = bill.BillingMonth,
                Status = Enum.Parse<BillStatus>(bill.Status),
                CreatedAt = bill.CreatedAt
            });
        }
    }
}
