using BillingService.Application.Abstractions;
using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using CredVault.Shared.Contracts.Billing.Events;
using CredVault.Shared.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Commands.GenerateBill
{
    public class GenerateBillCommandHandler
    : IRequestHandler<GenerateBillCommand, ApiResponse<Guid>>
    {
        private readonly IBillRepository _billRepo;
        private readonly IEventPublisher _events;
        public GenerateBillCommandHandler(
            IBillRepository billRepo, IEventPublisher events)
        {
            _billRepo = billRepo;
            _events = events;
        }

        public async Task<ApiResponse<Guid>> Handle(
        GenerateBillCommand request, CancellationToken ct)
        {
            // Check if bill already exists for this card/month
            var existing = await _billRepo.GetByCardAndMonthAsync(
                request.CardId, request.BillingMonth, ct);
            if (existing is not null)
                return ApiResponse<Guid>.FailureResponse(
                    $"Bill already exists for {request.BillingMonth}.");

            var bill = Bill.Create(
            request.UserId, request.CardId, request.TotalAmount,
            request.MinimumDue, request.DueDate, request.BillingMonth);

            await _billRepo.AddAsync(bill, ct);

            await _events.PublishAsync(new BillGeneratedEvent
            {
                BillId = bill.Id,
                UserId = bill.UserId,
                CardId = bill.CardId,
                TotalAmount = bill.TotalAmount,
                MinimumDue = bill.MinimumDue,
                DueDate = bill.DueDate,
                BillingMonth = bill.BillingMonth
            }, ct);

            return ApiResponse<Guid>.SuccessResponse(bill.Id, "Bill generated.");
        }
    }
}
