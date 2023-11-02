using MantaRays_Weather.Enums;
using MantaRays_Weather.Models;
using MantaRays_Weather.Models.Daily;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<ApiResult<T?>> GetForecastByZip<T>(string zip, ForecastType type) where T : class;

    }
}
