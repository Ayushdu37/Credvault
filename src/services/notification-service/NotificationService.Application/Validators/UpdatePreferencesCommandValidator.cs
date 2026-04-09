using FluentValidation;
using NotificationService.Application.Commands.UpdatePreferences;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Validators
{
    public class UpdatePreferencesCommandValidator
    : AbstractValidator<UpdatePreferencesCommand>
    {
        public UpdatePreferencesCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
