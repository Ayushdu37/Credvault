using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Payment?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<List<Payment>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Payment>> GetByBillIdAsync(Guid billId, Guid userId, CancellationToken ct = default);
        Task AddAsync(Payment payment, CancellationToken ct = default);
        Task UpdateAsync(Payment payment, CancellationToken ct = default);
    }
}
