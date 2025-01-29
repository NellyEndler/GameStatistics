using GameStatistics.DTO;
using GameStatistics.Models;

namespace GameStatistics.Interfaces
{
    public interface IGameStatisticsService
    {
		Task<UserInteraction>AddGameStatistics(UserInteractionRequest userInteraction, string userId);
		Task<string?>GetAverageVisits(string interactionPointName);
		Task<double>GetMedianTime(string interactionPointName);
		Task<Workshop?>DeleteStatistics(int id);
		Task<List<Workshop>?>GetAll();
		Task<Workshop?>UpdateWorkshop(UpdateWorkshopRquest dto, int id);
    }
}
