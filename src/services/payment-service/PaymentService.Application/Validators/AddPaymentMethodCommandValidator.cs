using FluentValidation;
using PaymentService.Application.Commands.AddPaymentMethod;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Validators
{
    public class AddPaymentMethodCommandValidator
    : AbstractValidator<AddPaymentMethodCommand>
    {
        public AddPaymentMethodCommandValidator()
        {
            RuleFor(x => x.MethodType).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Details).NotEmpty().MaximumLength(200);
        }
    }
}
