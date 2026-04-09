using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Commands.UpdatePreferences
{
    public record UpdatePreferencesCommand(
    Guid UserId,
    bool EmailEnabled,
    bool PaymentAlerts,
    bool BillReminders,
    bool RewardUpdates
) : IRequest<ApiResponse<NotificationPreferenceResponse>>;
}
