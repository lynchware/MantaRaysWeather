namespace MantaRays_Weather.Models.Hourly
{
    public class HourlyForecast
    {
        public object[] Context { get; set; }
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }
        public float[][][] Coordinates { get; set; }
    }

    public class Properties
    {
        public DateTime Updated { get; set; }
        public Period[] Periods { get; set; }
    }

    public class Period
    {
        public int Number { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDaytime { get; set; }
        public int Temperature { get; set; }
        public string TemperatureUnit { get; set; }
        public ProbabilityOfPrecipitation ProbabilityOfPrecipitation { get; set; }
        public Dewpoint Dewpoint { get; set; }
        public RelativeHumidity RelativeHumidity { get; set; }
        public string WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public string Icon { get; set; }
        public string ShortForecast { get; set; }
        public string DetailedForecast { get; set; }
    }

    public class ProbabilityOfPrecipitation
    {
        public string UnitCode { get; set; }
        public int Value { get; set; }
    }

    public class Dewpoint
    {
        public string UnitCode { get; set; }
        public float Value { get; set; }
    }

    public class RelativeHumidity
    {
        public string UnitCode { get; set; }
        public int Value { get; set; }
    }
}
