using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Abstractions
{
    /// <summary>
    /// Hashes card numbers using SHA256 for duplicate detection.
    /// Also extracts masked number from full card number.
    /// </summary>
    public interface ICardHasher
    {
        string HashCardNumber(string cardNumber);
        string MaskCardNumber(string cardNumber);
    }
}
