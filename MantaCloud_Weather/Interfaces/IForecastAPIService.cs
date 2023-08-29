using MantaRays_Weather.Models;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<OfficeGridPoints> GetGridPointsAsync(string lat, string lon);
        Task<GeoLocation> GetGeoLocation(string zip);
        Task<WeatherForecast> GetForecastAsync(string office, string gridx, string gridy);
    }
}
