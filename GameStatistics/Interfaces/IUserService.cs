using GameStatistics.DTO;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GameStatistics.Interfaces
{
    public interface IUserService
    {
        public Task <IdentityResult?> RegisterUser (UserDTO dto);
        public Task <IdentityResult?> RegisterAdmin(UserDTO dto);

        public Task <SignInResult?> Login (LoginDTO dto);
        public Task <string> GenerateJwtToken(ApplicationUser user);
        public Task <ApplicationUser?> GetUserByUsername (string username);
        public Task <List<ShowUserDTO>?> GetAllUsers(string? role);
        //public Task <List<ApplicationUser>?> GetAllAdmins();
        public Task<UpdateUserDTO?> UpdateUser(UpdateUserDTO dto, int id);
        public Task<IdentityResult?> DeleteUser (string id);
        public Task<IdentityResult?> DeleteUserAdmin (int id);
    }
}
