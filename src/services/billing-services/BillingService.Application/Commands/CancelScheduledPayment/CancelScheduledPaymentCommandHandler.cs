using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.CancelScheduledPayment
{
    public class CancelScheduledPaymentCommandHandler
    : IRequestHandler<CancelScheduledPaymentCommand, ApiResponse<bool>>
    {
        private readonly IPaymentScheduleRepository _scheduleRepo;
        public CancelScheduledPaymentCommandHandler(
            IPaymentScheduleRepository scheduleRepo)
            => _scheduleRepo = scheduleRepo;

        public async Task<ApiResponse<bool>> Handle(
        CancelScheduledPaymentCommand request, CancellationToken ct)
        {
            var schedule = await _scheduleRepo.GetByIdAndUserAsync(
                request.ScheduleId, request.UserId, ct);
            if (schedule is null)
                return ApiResponse<bool>.FailureResponse("Schedule not found.");

            if (schedule.Status != "Pending")
                return ApiResponse<bool>.FailureResponse(
                    "Only pending schedules can be cancelled.");

            schedule.Cancel();
            await _scheduleRepo.UpdateAsync(schedule, ct);

            return ApiResponse<bool>.SuccessResponse(true, "Schedule cancelled.");
        }
    }
}
