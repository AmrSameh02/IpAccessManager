namespace IPAccessManager.Application.Interfaces
{
	public interface IIpGeoLocationService
	{
		Task<string?> GetCountryCodeFromIpAsync(string ipAddress);

	}
}
