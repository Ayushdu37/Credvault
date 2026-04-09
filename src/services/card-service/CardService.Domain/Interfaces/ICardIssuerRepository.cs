using CardService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Interfaces
{
    public interface ICardIssuerRepository
    {
        Task<CardIssuer?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CardIssuer?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<List<CardIssuer>> GetAllAsync(CancellationToken ct = default);
        Task<CardIssuer?> DetectIssuerAsync(string cardNumber, CancellationToken ct = default);
    }
}
