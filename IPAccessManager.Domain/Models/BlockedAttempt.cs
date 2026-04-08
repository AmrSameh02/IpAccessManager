namespace IPAccessManager.Domain.Models
{
	public class BlockedAttempt
	{
		public string IpAddress { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public string CountryCode { get; set; } = string.Empty;
		public bool BlockedStatus { get; set; }
		public string UserAgent { get; set; } = string.Empty;
	}
}
