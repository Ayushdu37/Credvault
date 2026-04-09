using CardService.Application.Commands.VerifyCard;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Validators
{
    public class VerifyCardCommandValidator : AbstractValidator<VerifyCardCommand>
    {
        public VerifyCardCommandValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().WithMessage("Card ID is required.");

            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }
}
