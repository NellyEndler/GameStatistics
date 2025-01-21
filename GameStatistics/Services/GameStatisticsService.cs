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

        public async Task <Workshop> AddGameStatistics(WorkshopDTO dto)
		{
			Workshop workshop = new()
			{
				WorkShopVisits = dto.WorkShopVisits,
				TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds
			};
			_context.Workshops.Add(workshop);
			await _context.SaveChangesAsync();

			return workshop;
		}

		public async Task<double> GetAverageVisits()
		{
			if (!await _context.Workshops.AnyAsync())
				return 0;

			return await _context.Workshops
				.AverageAsync(w => w.WorkShopVisits);
		}

		public async Task<double> GetAverageTime()
		{
			if (!await _context.Workshops.AnyAsync())
				return 0;

			return await _context.Workshops
			   .AverageAsync(w => w.TotalWorkshopTimeInSeconds);
		}

		public async Task<double> GetMedianVisits()
		{
			var stats = await _context.Workshops.Select(w => w.WorkShopVisits).ToListAsync();

			if(stats == null)
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

		public async Task<Workshop?> UpdateWorkshop(UpdateWorkshopDTO dto)
		{
			var workshopStats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == dto.Id);
			if (workshopStats == null)
				return null;

			workshopStats.WorkShopVisits = dto.WorkShopVisits;
			workshopStats.TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds;
			//_context.Update(workshopStats);
			await _context.SaveChangesAsync();
			return workshopStats;
		}

        public Workshop UpdateWorkshop2(UpdateWorkshopDTO dto)
        {
            var workshopStats = _context.Workshops.FirstOrDefault(w => w.Id == dto.Id);
            if (workshopStats == null)
                return null;

            workshopStats.WorkShopVisits = dto.WorkShopVisits;
            workshopStats.TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds;
            //_context.Update(workshopStats);
            _context.SaveChanges();
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
