using CredVault.Shared.Contracts.Common;
using CredVault.Shared.Contracts.Identity.Responses;
using IdentityService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Queries.GetActiveSessions
{
    public class GetActiveSessionsQueryHandler
    : IRequestHandler<GetActiveSessionsQuery, ApiResponse<List<ActiveSessionResponse>>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public GetActiveSessionsQueryHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ApiResponse<List<ActiveSessionResponse>>> Handle(
        GetActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            var tokens = await _refreshTokenRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
            var sessions = tokens.Select(t => new ActiveSessionResponse
            {
                TokenId = t.Id,
                DeviceInfo = t.DeviceInfo,
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt
            }).ToList();
            return ApiResponse<List<ActiveSessionResponse>>.SuccessResponse(sessions);
        }
    }
}
