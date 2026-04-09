using CardService.Application.Commands.SetDefaultCard;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Validators
{
    public class SetDefaultCardCommandValidator : AbstractValidator<SetDefaultCardCommand>
    {
        public SetDefaultCardCommandValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().WithMessage("Card ID is required.");

            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }
}
