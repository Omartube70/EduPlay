using Application.Features.Users.Commands;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>Get all users in the system. Admin only.</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        /// <summary>Get the currently authenticated user's profile.</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetUserByIdQuery
            {
                TargetUserId = _currentUserService.UserId,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return Ok(result);
        }

        /// <summary>Get a single user by ID. Owner or Admin only.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery
            {
                TargetUserId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return Ok(result);
        }

        /// <summary>Update a user's username/email. Owner or Admin only.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var result = await _mediator.Send(new UpdateUserCommand
            {
                TargetUserId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin,
                UserName = dto.UserName,
                Email = dto.Email
            });

            return Ok(result);
        }

        /// <summary>Change a user's password. Owner (requires current password) or Admin.</summary>
        [HttpPost("{id:int}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            await _mediator.Send(new ChangePasswordCommand
            {
                TargetUserId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin,
                NewPassword = dto.NewPassword
            });

            return NoContent();
        }

        /// <summary>Delete a user account. Owner or Admin only.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteUserCommand
            {
                TargetUserId = id,
                CurrentUserId = _currentUserService.UserId,
                IsAdmin = _currentUserService.IsAdmin
            });

            return NoContent();
        }

        /// <summary>Promote a user to Admin role. Admin only.</summary>
        [HttpPost("{id:int}/promote")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promote(int id)
        {
            var result = await _mediator.Send(new PromoteToAdminCommand { TargetUserId = id });
            return Ok(result);
        }
    }
}