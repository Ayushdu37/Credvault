using CredVault.Shared.Contracts.Common;
using MediatR;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.RemovePaymentMethod
{
    public class RemovePaymentMethodCommandHandler
    : IRequestHandler<RemovePaymentMethodCommand, ApiResponse<bool>>
    {
        private readonly ISavedPaymentMethodRepository _methodRepo;
        public RemovePaymentMethodCommandHandler(
            ISavedPaymentMethodRepository methodRepo)
            => _methodRepo = methodRepo;

        public async Task<ApiResponse<bool>> Handle(
        RemovePaymentMethodCommand request, CancellationToken ct)
        {
            var method = await _methodRepo.GetByIdAndUserAsync(
                request.MethodId, request.UserId, ct);
            if (method is null)
                return ApiResponse<bool>.FailureResponse(
                    "Payment method not found.");

            await _methodRepo.DeleteAsync(method, ct);

            return ApiResponse<bool>.SuccessResponse(
                true, "Payment method removed.");
        }
    }
}
