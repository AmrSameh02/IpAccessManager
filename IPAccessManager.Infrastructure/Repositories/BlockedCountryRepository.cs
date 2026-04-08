using IPAccessManager.Application.Interfaces;
using IPAccessManager.Domain.Models;
using System.Collections.Concurrent;

namespace IPAccessManager.Infrastructure.Repositories
{
	public class BlockedCountryRepository : IBlockedCountryRepository
	{
		private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new(StringComparer.OrdinalIgnoreCase);

		public Task<bool> AddAsync(BlockedCountry country)
		{
			bool isAdded = _blockedCountries.TryAdd(country.CountryCode, country);
			return Task.FromResult(isAdded);
		}

		public Task<bool> DeleteAsync(string countryCode)
		{
			bool isRemoved = _blockedCountries.TryRemove(countryCode, out _);
			return Task.FromResult(isRemoved);
		}

		public Task<IEnumerable<BlockedCountry>> GetAllAsync(int page, int pageSize, string? searchTerm)
		{
			var query = _blockedCountries.Values.AsEnumerable();

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				query = query.Where(c =>
				c.CountryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
				(c.CountryName != null &&
				c.CountryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
			}

			var result = query.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			return Task.FromResult<IEnumerable<BlockedCountry>>(result);
		}

		public Task<bool> IsCountryBlockedAsync(string countryCode)
		{
			if (_blockedCountries.TryGetValue(countryCode, out var country))
			{
				if (country.IsExpired)
				{
					return Task.FromResult(false);
				}
				return Task.FromResult(true);
			}
			return Task.FromResult(false);
		}

		public Task RemoveExpiredBlockAsync()
		{
			foreach (var country in _blockedCountries)
			{
				if (country.Value.IsExpired)
				{
					_blockedCountries.TryRemove(country.Key, out _);
				}
			}
			return Task.CompletedTask;
		}
	}
}
