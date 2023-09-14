using MantaRays_Weather.Models;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<DailyForecast> GetWeatherForecastByZip(string zip);
    }
}
