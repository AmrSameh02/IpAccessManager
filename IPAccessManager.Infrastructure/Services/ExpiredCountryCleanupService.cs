using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IPAccessManager.Application.Interfaces;

namespace IPAccessManager.Infrastructure.Services
{
	public class ExpiredCountryCleanupService : BackgroundService
	{
		private readonly IBlockedCountryRepository _repository;
		private readonly ILogger<ExpiredCountryCleanupService> _logger;

		public ExpiredCountryCleanupService(IBlockedCountryRepository repository, ILogger<ExpiredCountryCleanupService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Cleanup Service: Checking for expired blocked countries...");

				try
				{
					await _repository.RemoveExpiredBlockAsync();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred while removing expired blocks.");
				}

				await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
			}
		}
	}
}