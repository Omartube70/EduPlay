using API.Services;
using Application.Features.Auth.Commands;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>Register a new user account.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), result);
        }

        /// <summary>Log in with email and password, returns access + refresh tokens.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Exchange a valid refresh token for a new access/refresh token pair.</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Revoke a user's refresh token (logout). Admins can revoke any user's token.</summary>
        [HttpPost("revoke/{targetUserId:int}")]
        [Authorize]
        public async Task<IActionResult> Revoke(int targetUserId)
        {
            await _mediator.Send(new RevokeTokenCommand
            {
                TargetUserId = targetUserId,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return NoContent();
        }
    }
}