using FluentValidation;
using IdentityService.Application.Commands.SendOTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Validators
{
    public class SendOTPValidator : AbstractValidator<SendOTPCommand>
    {
        public SendOTPValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Purpose)
                .IsInEnum().WithMessage("Invalid OTP purpose.");
        }
    }
}
