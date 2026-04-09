using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Interfaces
{
    public interface ISavedPaymentMethodRepository
    {
        Task<SavedPaymentMethod?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<SavedPaymentMethod?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<List<SavedPaymentMethod>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<SavedPaymentMethod?> GetDefaultByUserAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(SavedPaymentMethod method, CancellationToken ct = default);
        Task UpdateAsync(SavedPaymentMethod method, CancellationToken ct = default);
        Task DeleteAsync(SavedPaymentMethod method, CancellationToken ct = default);
    }
}
