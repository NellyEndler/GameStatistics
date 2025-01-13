using GameStatistics.DTO;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace GameStatistics.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> RegisterUser (UserDTO dto);
        public Task<IdentityResult?> RegisterAdmin(UserDTO dto);

        public Task<SignInResult?> Login (LoginDTO dto);
        public Task<string> GenerateJwtToken(ApplicationUser user);
        public Task<ApplicationUser?> GetUserByUsername (string username);
    }
}
