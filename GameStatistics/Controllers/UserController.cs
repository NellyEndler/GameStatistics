﻿using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
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

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Im alive!");
        }

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
        [Authorize]
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

        [HttpGet]
        [Authorize]
        public IActionResult Protected()
        {
            return Ok("Välkommen");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.LoginUser(loginDTO);

            if (result.Succeeded)
            {
                var user = await _service.GetUserByUsername(loginDTO.UserName);
                if (user == null)
                    return NotFound("User not found");

                var (token, refreshToken) = await _service.GenerateJwtToken(user);
                await _service.StoreRefreshToken(user, refreshToken);

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

        [HttpPost]
        public async Task <IActionResult> RefreshToken([FromBody] RefreshTokenDTO request)
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Invalid data");

            var user = await _service.GetUserById(request.UserId);

            if (user == null)
                return Unauthorized("Invalid user.");

            var isValid = await _service.ValidateRefreshToken(user, request.RefreshToken);
            if(!isValid)
                return Unauthorized();

            var newJwtToken = await _service.GenerateJwtToken(user);
            var newRefreshToken =  _service.GenerateRefreshToken();

            await _service.StoreRefreshToken(user, newRefreshToken);
            return Ok(new
            {
                AccessToken = newJwtToken.jwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [Authorize(Roles = "User")]
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
            var delete = await _service.DeleteService(currentUserRole, currentUserId, id);

            if (!delete)
                return NotFound($"$Could not find user with ID {id} to delete.");
            return Ok($"User with ID {id} successfully deleted.");
        }
    }
}
