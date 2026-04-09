using BillingService.Application.Commands.GenerateBill;
using BillingService.Application.Commands.SchedulePayment;
using BillingService.Application.Commands.CancelScheduledPayment;
using BillingService.Application.Queries.GetBills;
using BillingService.Application.Queries.GetBillById;
using BillingService.Application.Queries.GetBillsByCard;
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
    public class BillsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BillsController(IMediator mediator) => _mediator = mediator;
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Generate a bill for a card</summary>
        [HttpPost]
        public async Task<IActionResult> GenerateBill(
            [FromBody] GenerateBillRequest request)
        {
            var command = new GenerateBillCommand(
                GetUserId(), request.CardId, request.TotalAmount,
                request.MinimumDue, request.DueDate, request.BillingMonth);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get all bills for the logged-in user (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetBills(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetBillsQuery(GetUserId(), page, pageSize));
            return Ok(result);
        }

        /// <summary>Get a specific bill</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBillById(Guid id)
        {
            var result = await _mediator.Send(
                new GetBillByIdQuery(GetUserId(), id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get bills for a specific card</summary>
        [HttpGet("card/{cardId:guid}")]
        public async Task<IActionResult> GetBillsByCard(Guid cardId)
        {
            var result = await _mediator.Send(
                new GetBillsByCardQuery(GetUserId(), cardId));
            return Ok(result);
        }

        /// <summary>Schedule a payment for a bill</summary>
        [HttpPost("{billId:guid}/schedule")]
        public async Task<IActionResult> SchedulePayment(
            Guid billId, [FromBody] SchedulePaymentRequest request)
        {
            var command = new SchedulePaymentCommand(
                GetUserId(), billId, request.Amount, request.ScheduledDate);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Cancel a scheduled payment</summary>
        [HttpDelete("schedule/{scheduleId:guid}")]
        public async Task<IActionResult> CancelScheduledPayment(Guid scheduleId)
        {
            var command = new CancelScheduledPaymentCommand(
                GetUserId(), scheduleId);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
