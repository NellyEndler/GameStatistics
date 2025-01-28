using GameStatistics.Models;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GameStatistics.Context
{
    public class GameStatisticsContext : IdentityDbContext<ApplicationUser>
    {
        public GameStatisticsContext(DbContextOptions<GameStatisticsContext> options) : base(options) { }

        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<InteractionPoint> InteractionPoints { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserInteraction>(entity =>
            {
                entity.HasKey(ui => ui.Id);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(ui => ui.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ui => ui.InteractionPoint)
                      .WithMany()
                      .HasForeignKey(ui => ui.InteractionPointId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<InteractionPoint>(entity =>
            {
                entity.HasKey(ip => ip.Id);

                entity.Property(ip => ip.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });
        }
    }
}
