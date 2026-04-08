using IPAccessManager.Application.Interfaces;
using IPAccessManager.Domain.Models;
using System.Collections.Concurrent;

namespace IPAccessManager.Infrastructure.Repositories
{
	public class AttemptLogRepository : IAttemptLogRepository
	{
		private readonly ConcurrentQueue<BlockedAttempt> _logs = new();

		public Task AddLogAsync(BlockedAttempt attempt)
		{
			_logs.Enqueue(attempt);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<BlockedAttempt>> GetLogsAsync(int page, int pageSize)
		{
			var result = _logs
				.OrderByDescending(log => log.Timestamp)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			return Task.FromResult<IEnumerable<BlockedAttempt>>(result);
		}
	}
}
