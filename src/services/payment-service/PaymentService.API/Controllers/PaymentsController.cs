using PaymentService.Application.Commands.MakePayment;
using PaymentService.Application.Queries.GetPayments;
using PaymentService.Application.Queries.GetPaymentById;
using CredVault.Shared.Contracts.Enums;
using CredVault.Shared.Contracts.Payment.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator) => _mediator = mediator;
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Make a payment against a bill (Saga starts here)</summary>
        [HttpPost]
        public async Task<IActionResult> MakePayment(
            [FromBody] MakePaymentRequest request)
        {
            var methodName = ((PaymentMethodType)request.PaymentMethod).ToString();

            var command = new MakePaymentCommand(
                GetUserId(), request.BillId, request.CardId,
                request.Amount, methodName, request.TransactionReference);

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get all payments for the user (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetPayments(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(
                new GetPaymentsQuery(GetUserId(), page, pageSize));
            return Ok(result);
        }

        /// <summary>Get a specific payment</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _mediator.Send(
                new GetPaymentByIdQuery(GetUserId(), id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get payments for a specific bill</summary>
        [HttpGet("bill/{billId:guid}")]
        public async Task<IActionResult> GetPaymentsByBill(Guid billId)
        {
            var payments = await _mediator.Send(
                new GetPaymentsQuery(GetUserId()));
            // Filter by bill on top of user filter
            var filtered = payments.Data?.Items
                .Where(p => p.BillId == billId).ToList();
            return Ok(CredVault.Shared.Contracts.Common.ApiResponse<
                List<CredVault.Shared.Contracts.Payment.Responses.PaymentResponse>>
                .SuccessResponse(filtered ?? []));
        }
    }
}
