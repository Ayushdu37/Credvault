using BillingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Domain.Interfaces
{
    public interface IPaymentScheduleRepository
    {
        Task<PaymentSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PaymentSchedule?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<List<PaymentSchedule>> GetByBillIdAsync(Guid billId, CancellationToken ct = default);
        Task<List<PaymentSchedule>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<PaymentSchedule>> GetPendingByDateAsync(DateTime date, CancellationToken ct = default);
        Task AddAsync(PaymentSchedule schedule, CancellationToken ct = default);
        Task UpdateAsync(PaymentSchedule schedule, CancellationToken ct = default);
    }
}
