using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetBillsByCard
{
    public class GetBillsByCardQueryHandler
    : IRequestHandler<GetBillsByCardQuery, ApiResponse<List<BillResponse>>>
    {
        private readonly IBillRepository _billRepo;
        public GetBillsByCardQueryHandler(IBillRepository billRepo)
            => _billRepo = billRepo;

        public async Task<ApiResponse<List<BillResponse>>> Handle(
        GetBillsByCardQuery request, CancellationToken ct)
        {
            var bills = await _billRepo.GetByCardIdAsync(
                request.CardId, request.UserId, ct);

            var response = bills.Select(b => new BillResponse
            {
                Id = b.Id,
                CardId = b.CardId,
                TotalAmount = b.TotalAmount,
                MinimumDue = b.MinimumDue,
                AmountPaid = b.AmountPaid,
                DueDate = b.DueDate,
                BillingMonth = b.BillingMonth,
                Status = Enum.Parse<BillStatus>(b.Status),
                CreatedAt = b.CreatedAt
            }).ToList();

            return ApiResponse<List<BillResponse>>.SuccessResponse(response);
        }
    }
}
