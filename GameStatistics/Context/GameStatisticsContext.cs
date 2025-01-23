using GameStatistics.Models;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStatistics.Context
{
    public class GameStatisticsContext:IdentityDbContext<ApplicationUser>
    {
        public GameStatisticsContext(DbContextOptions<GameStatisticsContext> options) : base(options) { }

        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
