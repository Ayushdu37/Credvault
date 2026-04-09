using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Responses;
using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Queries.GetBills
{
    public class GetBillsQueryHandler
    : IRequestHandler<GetBillsQuery, ApiResponse<PaginatedResult<BillResponse>>>
    {
        private readonly IBillRepository _billRepo;
        public GetBillsQueryHandler(IBillRepository billRepo)
            => _billRepo = billRepo;

        public async Task<ApiResponse<PaginatedResult<BillResponse>>> Handle(
        GetBillsQuery request, CancellationToken ct)
        {
            var bills = await _billRepo.GetByUserIdAsync(request.UserId, ct);
            var allBills = bills.ToList();

            var totalCount = allBills.Count;
            var paged = allBills
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BillResponse
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

            var result = PaginatedResult<BillResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<BillResponse>>.SuccessResponse(result);
        }
    }
}
