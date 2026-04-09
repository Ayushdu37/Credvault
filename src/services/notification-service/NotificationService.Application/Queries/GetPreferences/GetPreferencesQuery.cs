using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Notification.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Queries.GetPreferences
{
    public record GetPreferencesQuery(Guid UserId)
    : IRequest<ApiResponse<NotificationPreferenceResponse>>;
}
