﻿namespace MantaRays_Weather.Models
{
    public class WeatherForecast
    {
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        public DateTime Updated { get; set; }
        public Period[] Periods { get; set; }
    }

}
