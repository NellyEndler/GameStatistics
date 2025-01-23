using GameStatistics.DTO;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GameStatistics.Interfaces
{
    public interface IUserService
    {
        Task <IdentityResult?> RegisterUser (UserDTO dto);
        Task <IdentityResult?> RegisterAdmin(UserDTO dto);
        Task <SignInResult> LoginUser (LoginDTO dto);
        Task <(string jwtToken, string refreshToken)> GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        Task StoreRefreshToken (ApplicationUser user, string refreshToken);
        Task <bool> ValidateRefreshToken (ApplicationUser user, string refreshToken);
        Task <ApplicationUser?> GetUserByUsername (string username);
        Task <ApplicationUser?> GetUserById (string id);
        Task <List<ShowUserDTO>?> GetAllUsers(string? role);
        //public Task <List<ApplicationUser>?> GetAllAdmins();
        Task<UpdateUserDTO?> UpdateUser(UpdateUserDTO dto, int id);
        Task<IdentityResult?> DeleteUser (string? id);
        Task<IdentityResult?> DeleteUserAdmin (int id);
        Task <bool> DeleteService (string? currentUserRole, string? currentUserId, int id);

/*        string CreateToken(ApplicationUser user);
*/    }
}
