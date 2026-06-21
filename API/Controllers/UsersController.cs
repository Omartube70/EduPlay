using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all users in the system. Admin only.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        /// <summary>Get a single user by ID. Admin only.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            return Ok(result);
        }

        /// <summary>Promote a user to Admin role. Admin only.</summary>
        [HttpPost("{id:int}/promote")]
        public async Task<IActionResult> Promote(int id)
        {
            var result = await _mediator.Send(new PromoteToAdminCommand { TargetUserId = id });
            return Ok(result);
        }
    }
}