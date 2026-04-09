using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
