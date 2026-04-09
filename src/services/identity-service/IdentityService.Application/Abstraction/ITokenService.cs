using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Abstraction
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
