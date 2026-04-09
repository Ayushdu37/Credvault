using CredVault.Shared.Contracts.Enums;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Persistence.Repositories
{
    public class OTPCodeRepository : IOTPCodeRepository
    {
        private readonly IdentityServiceDbContext _context;
        public OTPCodeRepository(IdentityServiceDbContext context)
        {
            _context = context;
        }

        public async Task<OTPCode?> GetLatestAsync(Guid userId, OTPPurpose purpose, CancellationToken cancellationToken = default)
        {
            return await _context.OTPCodes
                .Where(otp => otp.UserId == userId && otp.Purpose == purpose)
                .OrderByDescending(otp => otp.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(OTPCode otpCode, CancellationToken cancellationToken = default)
        {
            await _context.OTPCodes.AddAsync(otpCode, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkUsedAsync(OTPCode otpCode, CancellationToken cancellationToken = default)
        {
            otpCode.MarkUsed();
            _context.OTPCodes.Update(otpCode);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
