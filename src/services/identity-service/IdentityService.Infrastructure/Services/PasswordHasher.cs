using IdentityService.Application.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            // BCrypt automatically generates a salt and includes it in the hash
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool Verify(string passwordHash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
