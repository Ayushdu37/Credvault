using CredVault.Shared.Contracts.Enums;
using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Interfaces
{
    public interface IOTPCodeRepository
    {
        Task<OTPCode?> GetLatestAsync(Guid userId, OTPPurpose purpose, CancellationToken cancellationToken = default);
        Task AddAsync(OTPCode otpCode, CancellationToken cancellationToken = default);
        Task MarkUsedAsync(OTPCode otpCode, CancellationToken cancellationToken = default);
    }
}
