using BillingService.Application.Commands.RedeemRewards;
using BillingService.Application.Queries.GetRewardAccount;
using BillingService.Application.Queries.GetRewardTransactions;
using CredVault.Shared.Contracts.Billing.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RewardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RewardsController(IMediator mediator) => _mediator = mediator;
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get the user's reward account</summary>
        [HttpGet]
        public async Task<IActionResult> GetRewardAccount()
        {
            var result = await _mediator.Send(
                new GetRewardAccountQuery(GetUserId()));
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get reward transaction history (paginated)</summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(
                new GetRewardTransactionsQuery(GetUserId(), page, pageSize));
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Redeem reward points</summary>
        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemRewards(
            [FromBody] RedeemRewardsRequest request)
        {
            var command = new RedeemRewardsCommand(
                GetUserId(), request.PointsToRedeem);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
