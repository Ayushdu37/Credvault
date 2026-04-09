using CardService.Application.Commands.AddCard;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Validators
{
    public class AddCardCommandValidator : AbstractValidator<AddCardCommand>
    {
        public AddCardCommandValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches(@"^\d{15,16}$").WithMessage("Card number must be 15 or 16 digits.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Cardholder name is required.")
                .MaximumLength(200);

            RuleFor(x => x.ExpiryMonth)
                .InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12.");

            RuleFor(x => x.ExpiryYear)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage("Card has expired.");

            RuleFor(x => x.CreditLimit)
                .GreaterThan(0).WithMessage("Credit limit must be greater than zero.");

            RuleFor(x => x.BillingCycleStartDay)
                .InclusiveBetween(1, 28)
                .WithMessage("Billing cycle start day must be between 1 and 28.");

            RuleFor(x => x.Nickname)
                .MaximumLength(50).When(x => x.Nickname != null);
        }
    }
}
