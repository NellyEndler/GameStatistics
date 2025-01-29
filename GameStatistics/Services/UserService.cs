using GameStatistics.Context;
using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
#pragma warning disable CS8604 // Possible null reference argument.

namespace GameStatistics.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
         GameStatisticsContext contex, IConfiguration configuration, IHttpContextAccessor contextAccessor) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly GameStatisticsContext _context = contex;
        private readonly IConfiguration _configuration = configuration;

        public async Task<List<UserResponse>?> GetAllUsers(string? role)
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || users.Count == 0)
                return [];

            List<ApplicationUser> filteredUsers;

            if (string.IsNullOrEmpty(role))
                filteredUsers = users;
            else
                filteredUsers = users.Where(user => _userManager.IsInRoleAsync(user, role).Result).ToList();

            var userResponse = filteredUsers.Select(user => new UserResponse
            {
                Username = user.UserName,
            }).ToList();

            return userResponse;
        }

        public async Task<SignInResult> LoginUser(LoginRequest request)
        {

            var loginUser = await _userManager.FindByNameAsync(request.UserName);

            if (loginUser == null)
                return SignInResult.Failed;

            var loginResult = await _signInManager.CheckPasswordSignInAsync(loginUser, request.Password, false);

            if (loginResult.Succeeded)
                return SignInResult.Success;

            return SignInResult.Failed;
        }

        public async Task<bool> SignOut(string userId)
        {
            var refreshToken = await _context.RefreshTokens.Where(t => t.UserId == userId).FirstOrDefaultAsync();

            if (refreshToken == null)
                return false;

            refreshToken.Expires = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IdentityResult?> RegisterAdmin(UserRequest request)
        {
            var userModel = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(userModel, request.Password);
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

        public async Task<IdentityResult?> RegisterUser(UserRequest request)
        {
            var userModel = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(userModel, request.Password);
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

        public async Task<IdentityResult?> UpdateUser(UpdateUserRequest request, string? id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return null;

            var test1 = await _userManager.HasPasswordAsync(user);
            var test = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
                return null;

            return result;
        }

        public async Task<UpdateAdminRequest?> UpdateAdmin(UpdateAdminRequest request, string? id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return null;

            user.UserName = request.Username;
            user.Email = request.Email;

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, request.Role);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return null;

            var updatedUserDto = new UpdateAdminRequest
            {
                Username = user.UserName,
                Email = user.Email,
                Role = request.Role
            };

            return updatedUserDto;
        }

        public async Task<bool> DeleteService(string? currentUserRole, string? currentUserId, int id)
        {
            if (currentUserRole == "Admin")
            {
                var deleteAdmin = await DeleteUserAdmin(id);
                if (deleteAdmin == null)
                    return false;
                return true;
            }
            else
            {
                var deleteUser = await DeleteUser(currentUserId);
                if (deleteUser == null)
                    return false;
                return true;
            }
        }

        public async Task<IdentityResult?> DeleteUser(string? id)
        {
            var user = await _userManager.FindByIdAsync(id);

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

        public async Task<string> GetUserId(ApplicationUser user)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            return userId;
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
                new Claim("UserId", user.Id), // Lägg till användarens unika ID.
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
            await StoreRefreshToken(user, refreshToken);
            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task StoreRefreshToken(ApplicationUser user, string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Where(x => x.UserId.ToString() == user.Id)
                .FirstOrDefaultAsync();

            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.Expires = DateTime.Now.AddHours(1);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newToken = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    Expires = DateTime.Now.AddHours(1),
                };

                _context.RefreshTokens.Add(newToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateRefreshToken(ApplicationUser user, string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .FirstOrDefaultAsync();

            if(storedToken != null)
                if (storedToken.Expires <= DateTime.Now)
                    return false;

            return storedToken != null && storedToken.Token == refreshToken;
        }

    }
}
