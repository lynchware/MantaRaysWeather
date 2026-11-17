namespace MantaRays_Weather.Components;

public static class ForecastIconMapper
{
    public static string? GetIconPath(string? forecastDescription, bool isDaytime)
    {
        if (string.IsNullOrWhiteSpace(forecastDescription))
        {
            return null;
        }

        var forecast = forecastDescription.ToLowerInvariant();

        if (forecast.Contains("freezing rain")
            || forecast.Contains("freezing drizzle")
            || forecast.Contains("drizzle")
            || forecast.Contains("hail")
            || forecast.Contains("smoke")
            || forecast.Contains("dust")
            || forecast.Contains("tornado"))
        {
            return null;
        }

        if (forecast.Contains("sleet")
            || forecast.Contains("ice pellets")
            || (forecast.Contains("rain") && forecast.Contains("snow")))
        {
            return "Sleet_Icon.svg";
        }

        if (forecast.Contains("thunderstorm")
            || forecast.Contains("thunder")
            || forecast.Contains("t-storm"))
        {
            return "Storm_Icon.svg";
        }

        if (forecast.Contains("blizzard")
            || forecast.Contains("snowstorm")
            || forecast.Contains("blowing snow"))
        {
            return "Blizzard.svg";
        }

        if (forecast.Contains("snow"))
        {
            return "Flurries_Icon.svg";
        }

        if (forecast.Contains("wind") || forecast.Contains("breezy"))
        {
            return "wind.svg";
        }

        if (forecast.Contains("mist")
            || forecast.Contains("fog")
            || forecast.Contains("haze"))
        {
            return "Foggy_Icon.svg";
        }

        if (forecast.Contains("mostly cloudy") || forecast.Contains("partly sunny"))
        {
            return isDaytime ? "Mostly_Cloudy.svg" : "Night_Mostly_Cloudy.svg";
        }

        if (forecast.Contains("partly cloudy") || forecast.Contains("mostly sunny"))
        {
            return isDaytime ? "Partly_Cloudy_Icon.svg" : "Night_Mostly_Clear.svg";
        }

        if (forecast.Contains("cloudy"))
        {
            return "Cloudy_Icon.svg";
        }

        if (forecast.Contains("mostly clear"))
        {
            return isDaytime ? "Partly_Cloudy_Icon.svg" : "Night_Mostly_Clear.svg";
        }

        if (forecast.Contains("sunny") || forecast.Contains("clear"))
        {
            return isDaytime ? "Sunny.svg" : "Night_Clear.svg";
        }

        return null;
    }
}
