using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Payment.Responses;
using MediatR;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Queries.GetPaymentsMethods
{
    public class GetPaymentMethodsQueryHandler
    : IRequestHandler<GetPaymentMethodsQuery,
        ApiResponse<PaginatedResult<PaymentMethodResponse>>>
    {
        private readonly ISavedPaymentMethodRepository _methodRepo;
        public GetPaymentMethodsQueryHandler(
            ISavedPaymentMethodRepository methodRepo)
            => _methodRepo = methodRepo;

        public async Task<ApiResponse<PaginatedResult<PaymentMethodResponse>>> Handle(
        GetPaymentMethodsQuery request, CancellationToken ct)
        {
            var methods = await _methodRepo.GetByUserIdAsync(
            request.UserId, ct);

            var allMethods = methods.ToList();
            var totalCount = allMethods.Count;

            var paged = allMethods
                .OrderByDescending(m => m.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new PaymentMethodResponse
                {
                    Id = m.Id,
                    MethodType = m.MethodType,
                    DisplayName = m.DisplayName,
                    Details = m.Details,
                    IsDefault = m.IsDefault,
                    CreatedAt = m.CreatedAt
                }).ToList();

            var result = PaginatedResult<PaymentMethodResponse>.Create(
                paged, totalCount, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<PaymentMethodResponse>>
            .SuccessResponse(result);
        }
    }
}
