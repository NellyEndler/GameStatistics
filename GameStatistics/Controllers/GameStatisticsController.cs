using GameStatistics.DTO;
using GameStatistics.Interfaces;
using GameStatistics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStatistics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GameStatisticsController : ControllerBase
	{
		private readonly IGameStatisticsService _service;

		public GameStatisticsController(IGameStatisticsService service)
		{
			_service = service;
		}

		[HttpPost]
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

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var allStats = await _service.GetAll();
			if (allStats == null)
				return NotFound("No data available in database");
			return Ok(allStats);
		}

		[HttpGet]
		public async Task<IActionResult> GetStatistics()
		{
			var averageVisits = await _service.GetAverageVisits();
			var averageTime = await _service.GetAverageTime();

			var medianVisits = await _service.GetMedianVisits();

			if (averageVisits == 0 || averageTime == 0)
				return BadRequest("No data available to calculate statistics.");
			if (medianVisits == 0)
				return BadRequest("No data available to calculate statistics.");

			return Ok($"Average workshop visits: {averageVisits}" +
				$"\nMedian of workshop visits: {medianVisits}" +
				$"\nAverage time spent in workshop: {averageTime}");
		}

		[HttpPut]
		public async Task<IActionResult> UpdateStatistics([FromBody] UpdateWorkshopDTO dto)
		{
			if (dto == null || dto.Id <= 0)
				return BadRequest("Invalid workshop data.");

			var updatedStats = await _service.UpdateWorkshop(dto);
			if (updatedStats == null)
				return NotFound($"Workshop with ID {dto.Id} was not found");

			return Ok(updatedStats);
		}

		[HttpDelete]
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
