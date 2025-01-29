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

        public async Task<UserInteraction> AddGameStatistics(UserInteractionRequest dto, string userId)
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

            var medianVisits = await GetMedianTime(interactionPointName);

            string avarageStats = $"Avarage {interactionPointName} visits: {avarageVisists} \n" +
                $"Average time spent in {interactionPointName}: {avarageTime}\n" +
                $"Median of time spent in {interactionPointName}: {medianVisits}";
                
            return avarageStats;
        }

        public async Task<double> GetMedianTime(string interactionPointName)
        {
            var interactions = await _context.UserInteractions
                .Where(u => u.InteractionPoint.Name == interactionPointName)
                .Select(u => u.InteractionTimeInSeconds)
                .ToListAsync();

            if (interactions == null)
                return 0;

            interactions.Sort();
            int count = interactions.Count;

            //if count is odd, return the middle value
            if (count % 2 == 1)
            {
                var medianOdd = interactions[count/2];
                return medianOdd;
            }
            if (count == 2)
            {
                var middleValue = interactions[0] + interactions[1] / 2;
                return middleValue;
            }
            else
            {
                double middle1 = (count / 2) - 1;
                double middle2 = (count / 2);
                return (middle1 + middle2) / 2;
            }
        }

        public async Task<Workshop?> UpdateWorkshop(UpdateWorkshopRquest dto, int id)
        {
            var workshopStats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);

            if (workshopStats == null)
                return null;

            workshopStats.WorkShopVisits = dto.WorkShopVisits;
            workshopStats.TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds;
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
