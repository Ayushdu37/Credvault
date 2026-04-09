using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Abstraction
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string passwordHash, string password);
    }
}
