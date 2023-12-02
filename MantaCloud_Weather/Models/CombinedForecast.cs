using MantaRays_Weather.Models.Current;
using MantaRays_Weather.Models.Daily;
using MantaRays_Weather.Models.Hourly;

namespace MantaRays_Weather.Models
{
    public class CombinedForecast
    {
        public CurrentForecast? Current { get; set; }    
        public DailyForecast? Daily { get; set; }
        public HourlyForecast? Hourly { get; set; }
    }
}
