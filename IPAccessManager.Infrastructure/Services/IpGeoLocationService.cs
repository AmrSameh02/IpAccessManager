using IPAccessManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace IPAccessManager.Infrastructure.Services
{
	public class IpGeoLocationService : IIpGeoLocationService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly ILogger<IpGeoLocationService> _logger;

		public IpGeoLocationService(HttpClient httpClient, IConfiguration configuration, ILogger<IpGeoLocationService> logger)
		{
			_configuration = configuration;
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task<string?> GetCountryCodeFromIpAsync(string ipAddress)
		{
			try
			{
				var apiKey = _configuration["IpApi:Key"];

				string targetIp = (ipAddress == "::1" || ipAddress == "127.0.0.1" || string.IsNullOrWhiteSpace(ipAddress))
								  ? ""
								  : $"&ip={ipAddress}";

				var requestUrl = $"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}{targetIp}";

				var response = await _httpClient.GetAsync(requestUrl);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<IpApiResponse>();
					return result?.CountryCode;
				}
				else
				{
					var errorDetails = await response.Content.ReadAsStringAsync();
					_logger.LogWarning("API Error. Status: {StatusCode}, Reason: {Details}", response.StatusCode, errorDetails);
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while fetching geolocation.");
				return null;
			}
		}

		public class IpApiResponse
		{
			[System.Text.Json.Serialization.JsonPropertyName("country_code2")]
			public string? CountryCode { get; set; }
		}
	}
}