using GameStatistics.DTO;
using GameStatistics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameStatistics.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Im alive!");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterUser(dto);

            if (result != null)
                return Ok(result);
            else
                return BadRequest($"Failed to create user {result}");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterAdmin(dto);

            if (result != null)
                return Ok(result);
            else
                return BadRequest($"Failed to create user {result}");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Protected()
        {
            return Ok("Välkommen");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.LoginUser(request);

            if (result.Succeeded)
            {
                var user = await _service.GetUserByUsername(request.UserName);
                if (user == null)
                    return NotFound("User not found");

                var (token, refreshToken) = await _service.GenerateJwtToken(user);

                return Ok(new
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Message = "Login successfull"
                });
            }
            else
                return Unauthorized("Invalid login attempt");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Invalid data");

            var user = await _service.GetUserById(userId);

            if (user == null)
                return Unauthorized("Invalid user.");

            var isValid = await _service.ValidateRefreshToken(user, request.RefreshToken);

            if (!isValid)
                return Unauthorized();

            var (newAccessToken, newRefreshToken) = await _service.GenerateJwtToken(user);
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role)
        {
            var users = await _service.GetAllUsers(role);
            if (users == null || users.Count == 0)
                return NotFound("No users found.");
            return Ok(users);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SignOutUser()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await _service.SignOut(userId);
            if (!result)
                return NotFound("No refresh token found in database");

            return Ok("Signed out successfully");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest dto)
        {
            if (dto == null)
                return BadRequest("Invalid user data.");

            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
                return Unauthorized();

            var updatedUser = await _service.UpdateUser(dto, userId);

            if (updatedUser == null || !updatedUser.Succeeded)
                return BadRequest("Could not update user.");

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminRequest dto, string userId)
        {
            if (dto == null)
                return BadRequest("Invalid user data");

            var updatedUser = await _service.UpdateAdmin(dto, userId);

            if (updatedUser == null)
                return NotFound($"User with ID {userId} was not found.");
            return Ok($"Updated fields:\n{updatedUser}");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest();

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = User.FindFirst("UserID")?.Value;
            var delete = await _service.DeleteService(currentUserRole, currentUserId, id);

            if (!delete)
                return NotFound($"$Could not find user with ID {id} to delete.");
            return Ok($"User with ID {id} successfully deleted.");
        }
    }
}
