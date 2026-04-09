using FluentValidation;
using IdentityService.Application.Commands.VerifyOTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Validators
{
    public class VerifyOTPValidator : AbstractValidator<VerifyOTPCommand>
    {
        public VerifyOTPValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.OTPCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be 6 digits.");
            RuleFor(x => x.Purpose)
                .IsInEnum().WithMessage("Invalid OTP purpose.");
        }
    }
}
