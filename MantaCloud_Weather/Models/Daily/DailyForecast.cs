namespace MantaRays_Weather.Models.Daily
{
    public class DailyForecast
    {
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        public DateTime Updated { get; set; }
        public Period[] Periods { get; set; }
    }

}
