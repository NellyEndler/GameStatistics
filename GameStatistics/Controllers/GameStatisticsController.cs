using GameStatistics.DTO;
using GameStatistics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameStatistics.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameStatisticsController(IGameStatisticsService service) : ControllerBase
    {
        private readonly IGameStatisticsService _service = service;

        /*		[HttpPost]
				public async Task<IActionResult> SaveGameStatistics([FromBody] WorkshopDTO workshopStats)
				{
					if (workshopStats == null)
						return BadRequest();
					if(workshopStats.WorkShopVisits == 0 || workshopStats.TotalWorkshopTimeInSeconds == 0)
						return BadRequest("All values in the workshop statistics are 0. Cannot save to the database.");
					else
					{
						var savedStats = await _service.AddGameStatistics(workshopStats);
						return Ok(savedStats);
					}
				}		
				*/
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveGameStatistics([FromBody] UserInteractionRequest userInteraction)
        {
            if (userInteraction == null)
                return BadRequest("Invalid parameters.");
            if (userInteraction.InteractionTimeInSeconds == 0)
                return BadRequest("Interaction time is 0. Cannot save to the database.");
            else
            {
                var userId = User.FindFirst("UserId")?.Value;
                var savedStats = await _service.AddGameStatistics(userInteraction, userId);
                return Ok(savedStats);
            }
        }

        [HttpGet]
       // [Authorize]
        public async Task<IActionResult> GetStatistics([FromQuery] string interactionPointName)
        {
            var statistics = await _service.GetAverageVisits(interactionPointName);
            if (statistics == null)
                return BadRequest("No data available to calculate statistics.");
            return Ok(statistics);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatistics([FromBody] UpdateWorkshopRquest dto, int id)
        {
            if (dto == null || id <= 0)
                return BadRequest("Invalid workshop data.");

            var updatedStats = await _service.UpdateWorkshop(dto, id);
            if (updatedStats == null)
                return NotFound($"Workshop with ID {id} was not found");

            return Ok(updatedStats);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatistics(int id)
        {
            var stats = await _service.DeleteStatistics(id);

            if (stats != null)
                return Ok($"Statistics with ID {id} successfully got deleted.");
            else
                return NotFound($"Statistics with ID {id} does not exist.");
        }
    }
}
