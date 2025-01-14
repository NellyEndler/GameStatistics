using GameStatistics.DTO;
using GameStatistics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStatistics.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody]UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterUser(dto);

            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result.Errors);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> RegisterAdmin([FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _service.RegisterAdmin(dto);

            if (result.Succeeded)
                return Ok(result);
            else 
                return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Login ([FromBody] LoginDTO loginDTO)
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
    }
}
