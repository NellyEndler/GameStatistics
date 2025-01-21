using GameStatistics.Context;
using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GameStatistics.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
         GameStatisticsContext contex, IConfiguration configuration, IHttpContextAccessor contextAccessor) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly GameStatisticsContext _context = contex;
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


        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUser(LoginDTO dto)
        {
           // var loginUser1 = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password, false, false);

            var loginUser = await _userManager.FindByNameAsync(dto.UserName);

            if (loginUser == null)
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;

            var loginResult = await _signInManager.CheckPasswordSignInAsync(loginUser, dto.Password, false);
            //var validPassword = await _userManager.CheckPasswordAsync(loginUser, dto.Password);

            if(loginResult != null)
                return Microsoft.AspNetCore.Identity.SignInResult.Success;

            return Microsoft.AspNetCore.Identity.SignInResult.Failed;
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

        public async Task<ApplicationUser?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync($"{id}");
            if (user == null)
                return null;
            return user;
        }

        public async Task<(string jwtToken, string refreshToken)> GenerateJwtToken(ApplicationUser user)
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
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Skapar själva JWT-token med nödvändiga inställningar.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Issuer (vem som skapade och signerade token).
                audience: _configuration["Jwt:Audience"], // Audience (vilka som får använda token).
                claims: claims, // De claims som token innehåller.
                expires: DateTime.Now.AddMinutes(1), // Tokenens giltighetstid  ÄNDRA!!!!
                signingCredentials: creds // Signeringsuppgifterna för att verifiera tokenens äkthet.
            );

            var refreshToken = GenerateRefreshToken();

            // Returnerar den genererade JWT-token som en sträng.
            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }

        public string CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task StoreRefreshToken(ApplicationUser user, string refreshToken)
        {
            var existingToken = await _context.UserTokens
                .FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == "RefreshToken");

            if (existingToken != null)
                existingToken.Value = refreshToken;
            else
            {
                var newToken = new IdentityUserToken<string>
                {
                    UserId = user.Id,
                    LoginProvider = "RefreshToken",
                    Name = "RefreshToken",
                    Value = refreshToken
                };
                _context.UserTokens.Add(newToken);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<bool> ValidateRefreshToken(ApplicationUser user, string refreshToken)
        {
            var storedToken = await _context.UserTokens
                .FirstOrDefaultAsync(t => t.UserId == user.Id && t.LoginProvider == "RefreshToken");

            return storedToken != null && storedToken.Value == refreshToken;
        }
    }
}
