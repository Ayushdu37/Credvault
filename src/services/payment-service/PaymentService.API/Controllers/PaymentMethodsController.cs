using PaymentService.Application.Commands.AddPaymentMethod;
using PaymentService.Application.Commands.RemovePaymentMethod;
using PaymentService.Application.Queries.GetPaymentsMethods;
using CredVault.Shared.Contracts.Enums;
using CredVault.Shared.Contracts.Payment.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentMethodsController(IMediator mediator)
            => _mediator = mediator;
        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get saved payment methods (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(
                new GetPaymentMethodsQuery(GetUserId(), page, pageSize));
            return Ok(result);
        }

        /// <summary>Add a new payment method</summary>
        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod(
            [FromBody] AddPaymentMethodRequest request)
        {
            var typeName = ((PaymentMethodType)request.MethodType).ToString();
            var command = new AddPaymentMethodCommand(
                GetUserId(), typeName,
                request.DisplayName, request.Details);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Remove a saved payment method</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemovePaymentMethod(Guid id)
        {
            var command = new RemovePaymentMethodCommand(GetUserId(), id);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
