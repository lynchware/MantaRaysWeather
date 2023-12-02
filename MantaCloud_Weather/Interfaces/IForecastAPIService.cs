using MantaRays_Weather.Enums;
using MantaRays_Weather.Models;
using MantaRays_Weather.Models.Daily;

namespace MantaRays_Weather.Interfaces
{
    public interface IForecastAPIService
    {
        Task<ApiResult> GetForecastByZip(string zip);

    }
}
