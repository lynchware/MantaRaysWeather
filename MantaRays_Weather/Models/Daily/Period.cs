using System.Diagnostics.Metrics;

namespace MantaRays_Weather.Models.Daily
{
    public class Period
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDaytime { get; set; }
        public int Temperature { get; set; }
        public string TemperatureUnit { get; set; }
        public Measurement ProbabilityOfPrecipitation { get; set; }
        public Measurement Dewpoint { get; set; }
        public Measurement RelativeHumidity { get; set; }
        public string WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public string Icon { get; set; }
        public string ShortForecast { get; set; }
        public string DetailedForecast { get; set; }
    }
    public class Measurement
    {
        public string UnitCode { get; set; }
        public double? Value { get; set; }
    }
}
