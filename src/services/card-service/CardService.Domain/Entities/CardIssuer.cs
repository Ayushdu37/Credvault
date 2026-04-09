using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Domain.Entities
{
    /// <summary>
    /// Lookup table — seeded at database init.
    /// Used to auto-detect card type from BIN prefix and validate card number length.
    /// </summary>
    public class CardIssuer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int CardLength { get; private set; }
        public string BinPrefixes { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        // Navigation property
        public ICollection<CreditCard> CreditCards { get; private set; } = [];

        private CardIssuer() { }

        /// <summary>
        /// Used for seeding data only.
        /// </summary>
        public static CardIssuer Create(
        Guid id, string name, int cardLength, string binPrefixes)
        {
            return new CardIssuer
            {
                Id = id,
                Name = name,
                CardLength = cardLength,
                BinPrefixes = binPrefixes,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
