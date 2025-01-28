using GameStatistics.Context;
using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStatistics.Services
{
    public class GameStatisticsService(GameStatisticsContext context) : IGameStatisticsService
    {
        private readonly GameStatisticsContext _context = context;

        public async Task<List<Workshop>?> GetAll()
        {
            var allStats = await _context.Workshops.ToListAsync();
            if (allStats == null)
                return null;
            else
                return allStats;
        }

        public async Task<UserInteraction> AddGameStatistics(UserInteractionDTO dto, string userId)
        {
            UserInteraction userInteraction = new()
            {
                UserId = userId,
                InteractionPointId = dto.InteractionPointId,
                InteractionTimeInSeconds = dto.InteractionTimeInSeconds,
                TimeStamp = DateTime.Now
            };
            _context.UserInteractions.Add(userInteraction);
            await _context.SaveChangesAsync();

            return userInteraction;
        }

        public async Task<string?> GetAverageVisits(string interactionPointName)
        {
            var avarageTime = await _context.UserInteractions
                .Where(u => u.InteractionPoint.Name == interactionPointName)
                .AverageAsync(u => u.InteractionTimeInSeconds);

            var avarageVisists = await _context.UserInteractions
                .Where(u => u.InteractionPoint.Name == interactionPointName)
                .CountAsync();

                

           /* if (!await _context.Workshops.AnyAsync())
                return null;

            var visits = await _context.Workshops
                .AverageAsync(w => w.WorkShopVisits);
            var time = await _context.Workshops
                .AverageAsync(w => w.TotalWorkshopTimeInSeconds);
            var medianVisits = await GetMedianVisits();*/

            string avarageStats = $"Avarage workshop visits: {avarageVisists} \n" +
                $"Median of workshop visits: {medianVisits}\n" +
                $"Average time spent in workshop: {avarageTime}";
            return avarageStats;
        }

        public async Task<double> GetMedianVisits(string interactionPointName)
        {
            //var stats = await _context.Workshops.Select(w => w.WorkShopVisits).ToListAsync();
            var stats = await _context.UserInteractions
                .Where(u => u.InteractionPoint.Name == interactionPointName)
                .ToListAsync();

            if (stats == null)
                return 0;

            stats.Sort();
            int count = stats.Count;

            //if count is odd, return the middle value
            if (count % 2 == 1)
                return stats[count / 2];
            else
            {
                //If count is even, calculate the average of the two middle numbers
                double middle1 = stats[(count / 2) - 1];
                double middle2 = stats[(count / 2)];
                return (middle1 + middle2) / 2;
            }
        }

        public async Task<Workshop?> UpdateWorkshop(UpdateWorkshopDTO dto, int id)
        {
            var workshopStats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
            if (workshopStats == null)
                return null;

            workshopStats.WorkShopVisits = dto.WorkShopVisits;
            workshopStats.TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds;
            //_context.Update(workshopStats);
            await _context.SaveChangesAsync();
            return workshopStats;
        }

        public async Task<Workshop?> DeleteStatistics(int id)
        {
            var stats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);

            if (stats != null)
            {
                _context.Workshops.Remove(stats);
                await _context.SaveChangesAsync();
                return stats;
            }
            else
                return null;
        }
    }
}
