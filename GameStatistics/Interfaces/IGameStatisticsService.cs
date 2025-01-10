using GameStatistics.DTO;
using GameStatistics.Models;

namespace GameStatistics.Interfaces
{
    public interface IGameStatisticsService
    {
		Task AddGameStatistics(WorkshopDTO workshopStats);
		Task<double> GetAverageVisits();
		Task<double> GetAverageTime();
		Task<string> DeleteStatistics(int id);
		Task<Workshop?> UpdateWorkshop(WorkshopDTO dto, int id);
	}
}
