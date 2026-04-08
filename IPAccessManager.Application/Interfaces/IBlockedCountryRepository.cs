using IPAccessManager.Domain.Models;

namespace IPAccessManager.Application.Interfaces
{
	public interface IBlockedCountryRepository
	{
		Task<bool> AddAsync(BlockedCountry country);
		Task<bool> DeleteAsync(string countryCode);
		Task<IEnumerable<BlockedCountry>> GetAllAsync(int page, int pageSize, string? searchTerm);
		Task<bool> IsCountryBlockedAsync(string countryCode);
		Task RemoveExpiredBlockAsync();

	}
}
