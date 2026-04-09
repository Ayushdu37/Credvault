using CardService.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CardService.Infrastructure.Services
{

    public class CardHasher : ICardHasher
    {
        /// <summary>
        /// SHA256 hash of the card number for duplicate detection.
        /// </summary>
        public string HashCardNumber(string cardNumber)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(cardNumber.Trim()));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Masks all digits except the last 4.
        /// "4111111111111234" → "**** **** **** 1234"
        /// </summary>
        public string MaskCardNumber(string cardNumber)
        {
            var clean = cardNumber.Trim().Replace(" ", "");
            var last4 = clean[^4..];
            return $"**** **** **** {last4}";
        }
    }
}
