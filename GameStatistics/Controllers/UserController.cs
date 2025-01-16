using GameStatistics.DTO;
using GameStatistics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameStatistics.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterUser(dto);

            if (result != null)
                return Ok(result);
            else
                return BadRequest($"Failed to create user {result}");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterAdmin(dto);

            if (result != null)
                return Ok(result);
            else
                return BadRequest($"Failed to create user {result}");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.Login(loginDTO);

            if (result != null)
            {
                var user = await _service.GetUserByUsername(loginDTO.UserName);
                if (user == null)
                    return NotFound("User not found");

                var token = await _service.GenerateJwtToken(user);
                return Ok(new
                {
                    Token = token,
                    Message = "Login successfull"
                });
            }
            else
                return Unauthorized("Invalid login attempt");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role)
        {
            var users = await _service.GetAllUsers(role);
            if (users == null || users.Count == 0)
                return NotFound("No users found.");
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO dto, int id)
        {
            if (dto == null || id <= 0)
                return BadRequest("Invalid user data.");

            var updatedUser = await _service.UpdateUser(dto, id);
            if (updatedUser == null)
                return BadRequest("Could not update user.");
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task <IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest();

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserRole == "Admin")
            {
                var deleteUserAdmin = await _service.DeleteUserAdmin(id);
                if (deleteUserAdmin == null)
                    return NotFound($"Could not find user with ID {id} to delete.");
                return Ok($"User with ID {id} successfully deleted.");
            }
            else
            {
                var deleteUser = await _service.DeleteUser(currentUserId);
                if(deleteUser == null)
                    return NotFound($"Could not find user with ID {id} to delete.");
                return Ok($"User with ID {id} successfully deleted.");
            }

        }
    }
}
