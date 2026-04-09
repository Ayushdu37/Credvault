using CardService.Application.Commands.UpdateCardLimit;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Validators
{
    public class UpdateCardLimitCommandValidator : AbstractValidator<UpdateCardLimitCommand>
    {
        public UpdateCardLimitCommandValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().WithMessage("Card ID is required.");

            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.NewCreditLimit)
                .GreaterThan(0).WithMessage("Credit limit must be greater than zero.");
        }
    }
}
