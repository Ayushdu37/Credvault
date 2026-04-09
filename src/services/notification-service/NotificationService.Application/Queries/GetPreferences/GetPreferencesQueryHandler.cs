using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetPreferences
{
    public class GetPreferencesQueryHandler
    : IRequestHandler<GetPreferencesQuery,
        ApiResponse<NotificationPreferenceResponse>>
    {
        private readonly INotificationPreferenceRepository _prefRepo;
        public GetPreferencesQueryHandler(
            INotificationPreferenceRepository prefRepo)
            => _prefRepo = prefRepo;

        public async Task<ApiResponse<NotificationPreferenceResponse>> Handle(
        GetPreferencesQuery request, CancellationToken ct)
        {
            var prefs = await _prefRepo.GetByUserIdAsync(
            request.UserId, ct);

            if (prefs is null)
            {
                // Return defaults if no preferences set yet
                return ApiResponse<NotificationPreferenceResponse>
                    .SuccessResponse(new NotificationPreferenceResponse
                    {
                        EmailEnabled = true,
                        PaymentAlerts = true,
                        BillReminders = true,
                        RewardUpdates = true
                    });
            }

            return ApiResponse<NotificationPreferenceResponse>
            .SuccessResponse(new NotificationPreferenceResponse
            {
                EmailEnabled = prefs.EmailEnabled,
                PaymentAlerts = prefs.PaymentAlerts,
                BillReminders = prefs.BillReminders,
                RewardUpdates = prefs.RewardUpdates
            });
        }
    }
}
