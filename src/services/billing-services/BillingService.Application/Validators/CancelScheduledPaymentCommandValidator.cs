using BillingService.Application.Commands.CancelScheduledPayment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Validators
{
    public class CancelScheduledPaymentCommandValidator
    : AbstractValidator<CancelScheduledPaymentCommand>
    {
        public CancelScheduledPaymentCommandValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
