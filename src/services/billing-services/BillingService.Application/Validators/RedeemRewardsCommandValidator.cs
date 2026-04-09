using BillingService.Application.Commands.RedeemRewards;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Validators
{
    public class RedeemRewardsCommandValidator
    : AbstractValidator<RedeemRewardsCommand>
    {
        public RedeemRewardsCommandValidator()
        {
            RuleFor(x => x.PointsToRedeem).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
