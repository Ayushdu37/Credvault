using CardService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Interfaces
{
    public interface ICreditCardRepository
    {
        Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CreditCard?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<List<CreditCard>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<CreditCard?> GetDefaultByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<bool> ExistsByHashAsync(Guid userId, string cardNumberHash, CancellationToken ct = default);
        Task AddAsync(CreditCard card, CancellationToken ct = default);
        Task UpdateAsync(CreditCard card, CancellationToken ct = default);
    }
}
