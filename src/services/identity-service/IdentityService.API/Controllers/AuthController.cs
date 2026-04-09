using CredVault.Shared.Contracts.Identity.Requests;
using IdentityService.Application.Commands.LoginUser;
using IdentityService.Application.Commands.RefreshToken;
using IdentityService.Application.Commands.RegisterUser;
using IdentityService.Application.Commands.ResetPassword;
using IdentityService.Application.Commands.SendOTP;
using IdentityService.Application.Commands.VerifyEmail;
using IdentityService.Application.Commands.VerifyOTP;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // This makes the URL: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        // MediatR is injected — we send commands to it, it finds the right handler
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Register a new user</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {

            

            var command = new RegisterUserCommand(
                request.Email, request.Password, request.FullName, request.PhoneNumber);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Login with email and password</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {

            System.Diagnostics.Trace.WriteLine("***********************************************************");
            System.Diagnostics.Trace.WriteLine("Started " + DateTime.Now);

            // DeviceInfo comes from the User-Agent header (what browser/device they're using)
            var deviceInfo = Request.Headers.UserAgent.ToString();

            var command = new LoginUserCommand(request.Email, request.Password, deviceInfo);

            var result = await _mediator.Send(command);
            System.Diagnostics.Trace.WriteLine("***********************************************************");
            System.Diagnostics.Trace.WriteLine("Reached " + DateTime.Now);

            return result.Success ? Ok(result) : Unauthorized(result);
        }

        /// <summary>Get a new access token using a refresh token</summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand(request.RefreshToken);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : Unauthorized(result);
        }

        /// <summary>Verify email using OTP code</summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var command = new VerifyEmailCommand(request.Email, request.OTPCode);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Send an OTP code to the user's email</summary>
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequest request)
        {
            var command = new SendOTPCommand(request.Email, request.Purpose);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Verify an OTP code</summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            var command = new VerifyOTPCommand(request.Email, request.OTPCode, request.Purpose);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Reset password using OTP code</summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand(request.Email, request.OTPCode, request.NewPassword);

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
