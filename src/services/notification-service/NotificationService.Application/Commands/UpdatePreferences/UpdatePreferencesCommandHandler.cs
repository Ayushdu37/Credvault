using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.UpdatePreferences
{
    public class UpdatePreferencesCommandHandler
    : IRequestHandler<UpdatePreferencesCommand,
        ApiResponse<NotificationPreferenceResponse>>
    {
        private readonly INotificationPreferenceRepository _prefRepo;
        public UpdatePreferencesCommandHandler(
            INotificationPreferenceRepository prefRepo)
            => _prefRepo = prefRepo;

        public async Task<ApiResponse<NotificationPreferenceResponse>> Handle(
        UpdatePreferencesCommand request, CancellationToken ct)
        {
            var prefs = await _prefRepo.GetByUserIdAsync(request.UserId, ct);

            if (prefs is null)
            {
                prefs = NotificationPreference.CreateDefault(request.UserId);
                prefs.Update(request.EmailEnabled, request.PaymentAlerts,
                    request.BillReminders, request.RewardUpdates);
                await _prefRepo.AddAsync(prefs, ct);
            }
            else
            {
                prefs.Update(request.EmailEnabled, request.PaymentAlerts,
                    request.BillReminders, request.RewardUpdates);
                await _prefRepo.UpdateAsync(prefs, ct);
            }

            return ApiResponse<NotificationPreferenceResponse>.SuccessResponse(
            new NotificationPreferenceResponse
            {
                EmailEnabled = prefs.EmailEnabled,
                PaymentAlerts = prefs.PaymentAlerts,
                BillReminders = prefs.BillReminders,
                RewardUpdates = prefs.RewardUpdates
            }, "Preferences updated.");
        }
    }
}
