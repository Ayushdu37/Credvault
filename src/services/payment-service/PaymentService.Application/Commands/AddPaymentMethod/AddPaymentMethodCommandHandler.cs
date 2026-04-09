using CredVault.Shared.Contracts.Common;
using MediatR;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Commands.AddPaymentMethod
{
    public class AddPaymentMethodCommandHandler
    : IRequestHandler<AddPaymentMethodCommand, ApiResponse<Guid>>
    {
        private readonly ISavedPaymentMethodRepository _methodRepo;
        public AddPaymentMethodCommandHandler(
            ISavedPaymentMethodRepository methodRepo)
            => _methodRepo = methodRepo;

        public async Task<ApiResponse<Guid>> Handle(
        AddPaymentMethodCommand request, CancellationToken ct)
        {
            var method = SavedPaymentMethod.Create(
                request.UserId, request.MethodType,
                request.DisplayName, request.Details);

            // If it's the user's first method, make it default
            var existing = await _methodRepo.GetByUserIdAsync(
                request.UserId, ct);
            if (existing.Count == 0) method.SetAsDefault();

            await _methodRepo.AddAsync(method, ct);

            return ApiResponse<Guid>.SuccessResponse(
                method.Id, "Payment method added.");
        }
    }
}
