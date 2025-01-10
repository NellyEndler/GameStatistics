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
		public async Task<IActionResult> SaveGameStatistics([FromBody] WorkshopDTO workshopStats)
		{
			if (workshopStats == null)
				return BadRequest();
			else
			{
				await _service.AddGameStatistics(workshopStats);
				return Ok();
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAverageStatistics()
		{
			var averageVisits = await _service.GetAverageVisits();
			var averageTime = await _service.GetAverageTime();

			if (averageVisits == 0 || averageTime == 0)
				return BadRequest("No data available to calculate statistics.");

			return Ok($"Average workshop visits: {averageVisits}\nAverage time spent in workshop: {averageTime}");
		}

		[HttpPut]
		public async Task<IActionResult> UpdateStatistics([FromBody] WorkshopDTO dto, int id)
		{
			if (dto == null)
				return BadRequest();

			var updatedStats = await _service.UpdateWorkshop(dto, id);
			if (updatedStats == null)
				return BadRequest($"Workshop with ID {id} was not found");

			return Ok(updatedStats);
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteStatistics(int id)
		{
			string stats = await _service.DeleteStatistics(id);

			if (stats == "Deleted")
				return Ok(stats);
			else
				return BadRequest(stats);
		}
	}
}
