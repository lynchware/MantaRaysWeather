using MantaRays_Weather.Enums;
using MantaRays_Weather.Models.Daily;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<T> GetForecastByZip<T>(string zip, ForecastType type);

    }
}
