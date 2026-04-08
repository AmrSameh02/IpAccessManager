using IPAccessManager.Domain.Models;

namespace IPAccessManager.Application.Interfaces
{
	public interface IAttemptLogRepository
	{
		Task AddLogAsync(BlockedAttempt attempt);
		Task<IEnumerable<BlockedAttempt>> GetLogsAsync(int page, int pageSize);

	}
}
