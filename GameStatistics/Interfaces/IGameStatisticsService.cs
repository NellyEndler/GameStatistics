using GameStatistics.DTO;
using GameStatistics.Models;

namespace GameStatistics.Interfaces
{
    public interface IGameStatisticsService
    {
		Task <UserInteraction>AddGameStatistics(UserInteractionDTO userInteraction, string userId);
		Task <string?> GetAverageVisits(string interactionPointName);
		Task <double> GetAverageTime();
		Task <double> GetMedianVisits(string interactionPointName);
		Task<Workshop?> DeleteStatistics(int id);
		Task<List<Workshop>?> GetAll();
		Task<Workshop?> UpdateWorkshop(UpdateWorkshopDTO dto, int id);
    }
}
