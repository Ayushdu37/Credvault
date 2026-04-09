using BillingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Bill?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<List<Bill>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Bill>> GetByCardIdAsync(Guid cardId, Guid userId, CancellationToken ct = default);
        Task<Bill?> GetByCardAndMonthAsync(Guid cardId, string billingMonth, CancellationToken ct = default);
        Task AddAsync(Bill bill, CancellationToken ct = default);
        Task UpdateAsync(Bill bill, CancellationToken ct = default);
    }
}
