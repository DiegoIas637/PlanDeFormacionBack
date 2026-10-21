using Application.Users;
using Application.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlanDeFormacionBack.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("admins")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> CreateAdmin(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.CreateAdminAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateAdmin), new { id = response.Id }, response);
        }

        [HttpPost("clients")]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> CreateClient(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.CreateClientAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateClient), new { id = response.Id }, response);
        }
    }
}
