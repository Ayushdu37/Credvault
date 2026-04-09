using FluentValidation;
using PaymentService.Application.Commands.RemovePaymentMethod;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Validators
{
    public class RemovePaymentMethodCommandValidator
    : AbstractValidator<RemovePaymentMethodCommand>
    {
        public RemovePaymentMethodCommandValidator()
        {
            RuleFor(x => x.MethodId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
