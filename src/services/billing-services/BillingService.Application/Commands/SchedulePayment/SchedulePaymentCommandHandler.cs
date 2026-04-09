using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.SchedulePayment
{
    public class SchedulePaymentCommandHandler
    : IRequestHandler<SchedulePaymentCommand, ApiResponse<Guid>>
    {
        private readonly IBillRepository _billRepo;
        private readonly IPaymentScheduleRepository _scheduleRepo;
        public SchedulePaymentCommandHandler(
            IBillRepository billRepo, IPaymentScheduleRepository scheduleRepo)
        {
            _billRepo = billRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<ApiResponse<Guid>> Handle(
        SchedulePaymentCommand request, CancellationToken ct)
        {
            var bill = await _billRepo.GetByIdAndUserAsync(
            request.BillId, request.UserId, ct);
            if (bill is null || bill.IsDeleted)
                return ApiResponse<Guid>.FailureResponse("Bill not found.");

            if (bill.IsPaid)
                return ApiResponse<Guid>.FailureResponse("Bill is already paid.");

            if (request.ScheduledDate <= DateTime.UtcNow)
                return ApiResponse<Guid>.FailureResponse(
                    "Scheduled date must be in the future.");

            var schedule = PaymentSchedule.Create(
            request.BillId, request.UserId,
            request.Amount, request.ScheduledDate);

            await _scheduleRepo.AddAsync(schedule, ct);

            return ApiResponse<Guid>.SuccessResponse(
                schedule.Id, "Payment scheduled.");
        }
    }
}
