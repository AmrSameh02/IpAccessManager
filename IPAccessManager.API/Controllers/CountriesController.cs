using IPAccessManager.Application.Interfaces;
using IPAccessManager.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPAccessManager.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase
	{
		private readonly IBlockedCountryRepository _repository;

		public CountriesController(IBlockedCountryRepository repository)
		{
			_repository = repository;
		}

		[HttpPost("block")]
		public async Task<IActionResult> BlockCountry([FromBody] string countryCode)
		{
			if (string.IsNullOrWhiteSpace(countryCode))
				return BadRequest("Country code is required.");

			var country = new BlockedCountry { CountryCode = countryCode.ToUpper() };

			var isAdded = await _repository.AddAsync(country);

			if (!isAdded)
				return Conflict("Country is already blocked");

			return Ok("Country blocked Successfulyy");
		}

		[HttpDelete("block/{countryCode}")]
		public async Task<IActionResult> UnblockCountry(string countryCode)
		{
			var isRemoved = await _repository.DeleteAsync(countryCode);

			if (!isRemoved)
				return NotFound("Country is not found in the blocked list."); // التاسك طالب نرجع 404 لو مش موجودة

			return Ok("Country unblocked successfully.");
		}

		[HttpGet("blocked")]
		public async Task<IActionResult> GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
		{
			var countries = await _repository.GetAllAsync(page, pageSize, searchTerm);
			return Ok(countries);
		}

		[HttpPost("temporal-block")]
		public async Task<IActionResult> TemporalBlock([FromBody] TemporalBlockRequest request)
		{
			if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
				return BadRequest("Duration must be between 1 and 1440 minutes.");

			var country = new BlockedCountry
			{
				CountryCode = request.CountryCode.ToUpper(),
				ExpiryTime = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
			};

			var isAdded = await _repository.AddAsync(country);

			if (!isAdded)
				return Conflict("Country is already temporarily blocked.");

			return Ok($"Country {request.CountryCode} blocked temporarily for {request.DurationMinutes} minutes.");
		}
	}
	public class TemporalBlockRequest
	{
		public string CountryCode { get; set; } = string.Empty;
		public int DurationMinutes { get; set; }
	}
}
