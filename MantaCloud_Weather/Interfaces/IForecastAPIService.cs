using MantaRays_Weather.Models.Daily;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<DailyForecast> GetWeatherForecastByZip(string zip);
    }
}
