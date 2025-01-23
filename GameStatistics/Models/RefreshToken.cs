using Microsoft.AspNetCore.Identity;

namespace GameStatistics.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public required string UserId { get; set; }
        public DateTime Expires { get; set; }
    }
}