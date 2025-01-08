using GameStatistics.DTO;
using GameStatistics.Models;

namespace GameStatistics.Interfaces
{
    public interface IGameStatisticsService
    {
        Task AddGameStatistics(WorkshopDTO workshopStats);
        Task<double> GetAvarageVisits();
        Task<double> GetAverageTime();
    }
}
