using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameStatistics.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IHttpContextAccessor contextAccessor) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
        private readonly IConfiguration _configuration = configuration;

        public async Task<List<ShowUserDTO>?> GetAllUsers(string? role)
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || users.Count == 0)
                return new List<ShowUserDTO>();

            List<ApplicationUser> filteredUsers;

            if (string.IsNullOrEmpty(role))
                filteredUsers = users;
            else
                filteredUsers = users.Where(user => _userManager.IsInRoleAsync(user, role).Result).ToList();

            var userDtos = filteredUsers.Select(user => new ShowUserDTO
            {
                Username = user.UserName,
                Email = user.Email
            }).ToList();

            return userDtos;
        }

/*        public async Task<List<ApplicationUser>?> GetAllAdmins()
        {
            var adminUsers = new List<ApplicationUser>();
            var users = await _userManager.Users.ToListAsync();
            if (users == null)
                return null;

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    adminUsers.Add(user);
            }
            return adminUsers;
        }*/


        public async Task<Microsoft.AspNetCore.Identity.SignInResult?> Login(LoginDTO dto)
        {
            var loginUser = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password, false, false);
            if (!loginUser.Succeeded)
                return null;
            return loginUser;
        }

        public async Task<IdentityResult?> RegisterAdmin(UserDTO dto)
        {
            var userModel = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Username,
                Email = dto.Email,
            };

            var result = await _userManager.CreateAsync(userModel, dto.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(userModel, "Admin");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(userModel);
                    return null;
                }
            }
            return result;
        }

        public async Task<IdentityResult?> RegisterUser(UserDTO dto)
        {
            var userModel = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(userModel, dto.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(userModel, "User");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(userModel);
                    return null;
                }
            }
            return result;
        }

        public async Task<UpdateUserDTO?> UpdateUser(UpdateUserDTO dto, int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UserName = dto.Username;
            user.PasswordHash = dto.Username;
            user.Email = dto.Email;

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return null;

            var updatedUserDto = new UpdateUserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Password = user.PasswordHash,
                Email = user.Email,
                Role = dto.Role
            };

            return updatedUserDto;
        }

        public async Task<IdentityResult?> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return null;
            return result;
        }

        public async Task<IdentityResult?> DeleteUserAdmin(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return null;
            return result;
        }


        public async Task<ApplicationUser?> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            return user;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Hämtar claims kopplade till användaren från databasen.
            var userClaims = await _userManager.GetClaimsAsync(user);

            // Hämtar roller kopplade till användaren.
            var roles = await _userManager.GetRolesAsync(user);

            /*            if (string.IsNullOrEmpty(user.UserName) || (string.IsNullOrEmpty(user.Email)))
                            return null;
            */
            // Skapar en lista av claims som ska inkluderas i JWT-token.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Lägg till användarnamn som "subject" (standardfält i JWT).
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Skapar en unik identifierare för token.
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Lägg till användarens unika ID.
                new Claim(ClaimTypes.Email, user.Email) // Lägg till användarens e-postadress.
            };

            // Lägg till användarens roller som individuella claims.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Lägg till eventuella ytterligare claims kopplade till användaren.
            claims.AddRange(userClaims);

            // Skapar en säkerhetsnyckel baserat på den hemliga nyckeln definierad i appsettings.json.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // Definierar signeringsuppgifterna (HMAC-SHA256) med den säkerhetsnyckeln.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Skapar själva JWT-token med nödvändiga inställningar.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Issuer (vem som skapade och signerade token).
                audience: _configuration["Jwt:Audience"], // Audience (vilka som får använda token).
                claims: claims, // De claims som token innehåller.
                expires: DateTime.Now.AddHours(1), // Tokenens giltighetstid (1 timme).
                signingCredentials: creds // Signeringsuppgifterna för att verifiera tokenens äkthet.
            );

            // Returnerar den genererade JWT-token som en sträng.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
