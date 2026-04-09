using IdentityService.Application.Queries.GetActiveSessions;
using IdentityService.Application.Queries.GetUserById;
using IdentityService.Application.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // ALL endpoints in this controller require a valid JWT token
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get the current logged-in user's profile</summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Extract the user's ID from their JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetUserProfileQuery(userId));

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get a specific user by ID (Admin only)</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]  // Only users with Role = Admin can call this
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get all active sessions (refresh tokens) for the current user</summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetActiveSessionsQuery(userId));

            return Ok(result);
        }
    }
}
