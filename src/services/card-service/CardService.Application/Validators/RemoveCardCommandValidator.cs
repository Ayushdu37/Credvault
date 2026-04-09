using CardService.Application.Commands.RemoveCard;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Application.Validators
{
    public class RemoveCardCommandValidator : AbstractValidator<RemoveCardCommand>
    {
        public RemoveCardCommandValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().WithMessage("Card ID is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }
}
