namespace IPAccessManager.Domain.Models
{
	public class BlockedCountry
	{
		public string CountryCode { get; set; } = string.Empty;
		public string? CountryName { get; set; }
		public DateTime? ExpiryTime { get; set; }
		public bool IsExpired => ExpiryTime.HasValue && ExpiryTime.Value <= DateTime.UtcNow;

	}
}
