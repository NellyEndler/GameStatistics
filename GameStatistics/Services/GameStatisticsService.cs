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

		public async Task<string> DeleteStatistics(int id)
		{
			var stats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);

			if (stats != null)
			{
				_context.Workshops.Remove(stats);
				await _context.SaveChangesAsync();
				return "Deleted";
			}
			else
				return $"Statistics with ID {id} does not exist.";
		}

		public async Task<Workshop?> UpdateWorkshop(WorkshopDTO dto, int id)
		{
			var workshopStats = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == id);
			if (workshopStats == null)
				return null;

			workshopStats.WorkShopVisits = dto.WorkShopVisits;
			workshopStats.TotalWorkshopTimeInSeconds = dto.TotalWorkshopTimeInSeconds;
			_context.Update(workshopStats);
			await _context.SaveChangesAsync();
			return workshopStats;
		}
	}
}
