using FluentValidation;
using IdentityService.Application.Commands.RefreshToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Validators
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
