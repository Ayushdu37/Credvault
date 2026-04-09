using CardService.Application.Commands.AddCard;
using CardService.Application.Commands.RemoveCard;
using CardService.Application.Commands.SetDefaultCard;
using CardService.Application.Commands.UpdateCardLimit;
using CardService.Application.Commands.VerifyCard;
using CardService.Application.Queries.GetCardById;
using CardService.Application.Queries.GetCards;
using CardService.Application.Queries.GetCardUtilization;
using CredVault.Shared.Contracts.Card.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CardService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CardsController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// Gets the current user's ID from the JWT token.
        /// </summary>
        private Guid GetUserId()
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST /api/cards
        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] AddCardRequest request)
        {
            var command = new AddCardCommand(
                UserId: GetUserId(),
                CardNumber: request.CardNumber,
                CardHolderName: request.CardHolderName,
                ExpiryMonth: request.ExpiryMonth,
                ExpiryYear: request.ExpiryYear,
                Issuer: request.Issuer,
                CreditLimit: request.CreditLimit,
                BillingCycleStartDay: request.BillingCycleStartDay,
                Nickname: request.Nickname);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET /api/cards?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetCards(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetCardsQuery(GetUserId(), page, pageSize));
            return Ok(result);
        }

        // GET /api/cards/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCardById(Guid id)
        {
            var result = await _mediator.Send(
                new GetCardByIdQuery(GetUserId(), id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET /api/cards/utilization
        [HttpGet("utilization")]
        public async Task<IActionResult> GetUtilization()
        {
            var result = await _mediator.Send(
                new GetCardUtilizationQuery(GetUserId()));
            return Ok(result);
        }

        // PUT /api/cards/{id}/default
        [HttpPut("{id:guid}/default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _mediator.Send(
                new SetDefaultCardCommand(GetUserId(), id));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // PUT /api/cards/{id}/verify
        [HttpPut("{id:guid}/verify")]
        public async Task<IActionResult> Verify(Guid id)
        {
            var result = await _mediator.Send(
                new VerifyCardCommand(GetUserId(), id));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // PUT /api/cards/{id}/limit
        [HttpPut("{id:guid}/limit")]
        public async Task<IActionResult> UpdateLimit(
            Guid id, [FromBody] UpdateCardLimitRequest request)
        {
            var result = await _mediator.Send(
                new UpdateCardLimitCommand(GetUserId(), id, request.NewCreditLimit));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE /api/cards/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveCard(Guid id)
        {
            var result = await _mediator.Send(
                new RemoveCardCommand(GetUserId(), id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
