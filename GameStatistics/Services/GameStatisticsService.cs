using GameStatistics.Context;
using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStatistics.Services
{
    public class GameStatisticsService : IGameStatisticsService
    {
        private readonly GameStatisticsContext _context;

        public GameStatisticsService(GameStatisticsContext context)
        {
            _context = context;
        }

        public async Task AddGameStatistics(WorkshopDTO dto)
        {
            Workshop workshop = new()
            {
                WorkShopVisits = dto.WorkShopVisits,
                TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds
            };
            _context.Workshops.Add(workshop);
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAvarageVisits()
        {
            var avarageVisits = await _context.Workshops
                .AverageAsync(w => w.WorkShopVisits);

            return avarageVisits;
        }

        public async Task<double> GetAverageTime()
        {
            var avarageTime = await _context.Workshops
               .AverageAsync(w => w.TotalWorkshopTimeInSeconds);

            return avarageTime;
        }

        public async Task<List<Workshop>> GetAllStats()
        {
            var allStats = await _context.Workshops.ToListAsync();
            return allStats;
        }
    }
}
