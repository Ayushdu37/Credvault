using BillingService.Application.Commands.SchedulePayment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Validators
{
    public class SchedulePaymentCommandValidator
    : AbstractValidator<SchedulePaymentCommand>
    {
        public SchedulePaymentCommandValidator()
        {
            RuleFor(x => x.BillId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.ScheduledDate).GreaterThan(DateTime.UtcNow);
        }
    }
}
