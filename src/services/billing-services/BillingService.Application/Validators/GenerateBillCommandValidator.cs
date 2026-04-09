using BillingService.Application.Commands.GenerateBill;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Validators
{
    public class GenerateBillCommandValidator
    : AbstractValidator<GenerateBillCommand>
    {
        public GenerateBillCommandValidator()
        {
            RuleFor(x => x.CardId).NotEmpty();
            RuleFor(x => x.TotalAmount).GreaterThan(0);
            RuleFor(x => x.MinimumDue).GreaterThan(0)
                .LessThanOrEqualTo(x => x.TotalAmount);
            RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow);
            RuleFor(x => x.BillingMonth)
                .NotEmpty()
                .Matches(@"^\d{4}-\d{2}$")
                .WithMessage("BillingMonth must be YYYY-MM format.");
        }
    }
}
