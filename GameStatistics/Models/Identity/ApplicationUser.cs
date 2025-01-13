using Microsoft.AspNetCore.Identity;

namespace GameStatistics.Models.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
