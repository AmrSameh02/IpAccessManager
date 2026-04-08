using IPAccessManager.Application.Interfaces;
using IPAccessManager.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IPAccessManager.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IpController : ControllerBase
	{
		private readonly IIpGeoLocationService _ipService;
		private readonly IBlockedCountryRepository _countryRepository;
		private readonly IAttemptLogRepository _logRepository;

		public IpController(IIpGeoLocationService ipService,
							IBlockedCountryRepository countryRepository,
							IAttemptLogRepository logRepository)
		{
			_ipService = ipService;
			_countryRepository = countryRepository;
			_logRepository = logRepository;
		}

		[HttpGet("lookup")]
		public async Task<IActionResult> LookupIp([FromQuery] string? ipAddress)
		{
			string ipToLookup = ipAddress ?? string.Empty;

			if (string.IsNullOrWhiteSpace(ipToLookup))
			{
				ipToLookup = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

				if (string.IsNullOrWhiteSpace(ipToLookup))
					return BadRequest("Cannot determine IP address.");
			}
			else
			{
				if (!IPAddress.TryParse(ipToLookup, out _))
					return BadRequest("Invalid IP format.");
			}

			var countryCode = await _ipService.GetCountryCodeFromIpAsync(ipToLookup);

			if (countryCode == null)
				return StatusCode(500, "Could not fetch geolocation data from external API.");

			return Ok(new { IpAddress = ipToLookup, CountryCode = countryCode });
		}


		[HttpGet("check-block")]
		public async Task<IActionResult> CheckBlock()
		{
			string callerIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

			var countryCode = await _ipService.GetCountryCodeFromIpAsync(callerIp);
			if (countryCode == null)
				return StatusCode(500, "Could not fetch geolocation data.");

			bool isBlocked = await _countryRepository.IsCountryBlockedAsync(countryCode);

			if (isBlocked)
			{
				var userAgent = Request.Headers["User-Agent"].ToString();

				var attempt = new BlockedAttempt
				{
					IpAddress = callerIp,
					CountryCode = countryCode,
					BlockedStatus = true,
					UserAgent = userAgent
				};

				await _logRepository.AddLogAsync(attempt);

				return StatusCode(403, "Access Denied. Your country is blocked.");
			}

			return Ok("Access Granted. Your country is not blocked.");
		}

		[HttpGet("/api/logs/blocked-attempts")]
		public async Task<IActionResult> GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var logs = await _logRepository.GetLogsAsync(page, pageSize);
			return Ok(logs);
		}
	}
}