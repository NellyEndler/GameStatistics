using GameStatistics.DTO;
using GameStatistics.Models;

namespace GameStatistics.Interfaces
{
    public interface IGameStatisticsService
    {
		Task <Workshop>AddGameStatistics(WorkshopDTO workshopStats);
		Task <double> GetAverageVisits();
		Task <double> GetAverageTime();
		Task <double> GetMedianVisits();
		Task<Workshop?> DeleteStatistics(int id);
		Task<List<Workshop>?> GetAll();
		Task<Workshop?> UpdateWorkshop(UpdateWorkshopDTO dto);
		Workshop UpdateWorkshop2(UpdateWorkshopDTO dto);

    }
}
