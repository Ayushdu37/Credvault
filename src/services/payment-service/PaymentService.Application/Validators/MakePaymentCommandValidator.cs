using FluentValidation;
using PaymentService.Application.Commands.MakePayment;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Validators
{
    public class MakePaymentCommandValidator
    : AbstractValidator<MakePaymentCommand>
    {
        public MakePaymentCommandValidator()
        {
            RuleFor(x => x.BillId).NotEmpty();
            RuleFor(x => x.CardId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PaymentMethod).NotEmpty();
        }
    }
}
