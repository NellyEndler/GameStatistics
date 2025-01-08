using GameStatistics.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStatistics.Context
{
    public class GameStatisticsContext:DbContext
    {
        public GameStatisticsContext(DbContextOptions<GameStatisticsContext> options) : base(options) { }

        public DbSet<Workshop> Workshops { get; set; }
    }
}
