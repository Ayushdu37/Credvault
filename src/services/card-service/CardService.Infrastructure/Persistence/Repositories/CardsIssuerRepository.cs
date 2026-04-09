using CardService.Domain.Entities;
using CardService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure.Persistence.Repositories
{
    public class CardIssuerRepository : ICardIssuerRepository
    {
        private readonly CardServiceDbContext _context;
        public CardIssuerRepository(CardServiceDbContext context)
            => _context = context;

        public async Task<CardIssuer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CardIssuers.FindAsync([id], ct);

        public async Task<CardIssuer?> GetByNameAsync(
            string name, CancellationToken ct = default)
            => await _context.CardIssuers
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower(), ct);

        public async Task<List<CardIssuer>> GetAllAsync(CancellationToken ct = default)
            => await _context.CardIssuers.ToListAsync(ct);

        /// <summary>
        /// Detects the card issuer by checking BIN prefix.
        /// e.g. "4111111111111111" → starts with "4" → Visa
        /// </summary>
        public async Task<CardIssuer?> DetectIssuerAsync(
        string cardNumber, CancellationToken ct = default)
        {
            var issuers = await GetAllAsync(ct);

            foreach (var issuer in issuers)
            {
                var prefixes = issuer.BinPrefixes.Split(',',
                StringSplitOptions.TrimEntries);

                foreach (var prefix in prefixes)
                {
                    // Handle range prefixes like "2221-2720"
                    if (prefix.Contains('-'))
                    {
                        var parts = prefix.Split('-');
                        if (parts.Length == 2
                            && int.TryParse(parts[0], out var start)
                            && int.TryParse(parts[1], out var end))
                        {
                            var cardPrefix = cardNumber[..parts[0].Length];
                            if (int.TryParse(cardPrefix, out var cardNum)
                                && cardNum >= start && cardNum <= end)
                                return issuer;
                        }
                    }
                    else if (cardNumber.StartsWith(prefix))
                    {
                        return issuer;
                    }
                }
            }
            return null;
        }
    }
}
