using BillingService.Domain.Entities;
using BillingService.Domain.Interfaces;
using BillingService.Infrastructure.Persistence.Configurations;
using CredVault.Shared.Contracts.Payment.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Messaging.Consumers
{
    public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly IBillRepository _billRepo;
        private readonly IRewardRepository _rewardRepo;
        private readonly ILogger<PaymentCompletedConsumer> _logger;
        public PaymentCompletedConsumer(
            IBillRepository billRepo,
            IRewardRepository rewardRepo,
            ILogger<PaymentCompletedConsumer> logger)
        {
            _billRepo = billRepo;
            _rewardRepo = rewardRepo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation(
                "Processing PaymentCompleted: PaymentId={PaymentId}, BillId={BillId}, Amount={Amount}",
                msg.PaymentId, msg.BillId, msg.Amount);

            // 1. Update bill's AmountPaid
            var bill = await _billRepo.GetByIdAsync(msg.BillId);
            if (bill is not null)
            {
                bill.ApplyPayment(msg.Amount);
                await _billRepo.UpdateAsync(bill);
            }

            // 2. Earn reward points (1 point per ₹100 paid)
            var pointsEarned = (int)(msg.Amount / 100);
            if (pointsEarned <= 0) return;
            var account = await _rewardRepo.GetAccountByUserIdAsync(msg.UserId);
            if (account is null)
            {
                // First payment — create reward account with Silver tier
                account = RewardAccount.Create(
                    msg.UserId, RewardTierConfiguration.SilverId);
                await _rewardRepo.AddAccountAsync(account);
            }

            account.EarnPoints(pointsEarned);

            // Check for tier upgrade
            var newTier = await _rewardRepo.GetTierForPointsAsync(
                account.TotalEarned);
            if (newTier is not null && newTier.Id != account.TierId)
                account.UpdateTier(newTier.Id);

            await _rewardRepo.UpdateAccountAsync(account);

            var transaction = RewardTransaction.CreateEarned(
                account.Id, msg.PaymentId, pointsEarned,
                $"Earned {pointsEarned} pts on ₹{msg.Amount} payment");
            await _rewardRepo.AddTransactionAsync(transaction);
        }
    }
}
