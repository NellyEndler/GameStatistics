using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStatistics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStatisticsController : ControllerBase
    {
        private IGameStatisticsService _service;

        public GameStatisticsController(IGameStatisticsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task <IActionResult> SaveGameStatistics([FromBody]WorkshopDTO workshopStats)
        {
            await _service.AddGameStatistics(workshopStats);
            return Ok();
        }

        [HttpGet]
        public async Task <IActionResult> GetAvarageStatistics()
        {
            var avarageVisits = await _service.GetAvarageVisits();
            var avarageTime = await _service.GetAverageTime();
            return Ok($"Average workshop visits: {avarageVisits}\nAverage time spent in workshop: {avarageTime}");
        }

        [HttpGet]
        [Route("allstats")]
        public async Task <IActionResult> GetAllStatistics()
        {
            var allStats = await _service.GetAllStats();
            return Ok(allStats);
        }
    }
}
