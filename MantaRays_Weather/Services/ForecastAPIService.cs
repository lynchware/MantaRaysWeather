using MantaRays_Weather.Interfaces;
using System.Net.Http;
using MantaRays_Weather.Models.Daily;
using MantaRays_Weather.Models.Hourly;
using MantaRays_Weather.Models.Current; 
using MantaRays_Weather.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using MantaRays_Weather.Models;
using MantaRays_Weather.Models.Alerts;
using System.Text.Json;
using System.Globalization;

namespace MantaRays_Weather.Services;

public class ForecastAPIService : IForecastAPIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ForecastAPIService> _logger;
    private readonly string _geoApiKey;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ForecastAPIService(HttpClient httpClient, ILogger<ForecastAPIService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _logger = logger;
        _geoApiKey = configuration["APIs:GeoCode:Key"];
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }


    public async Task<ApiResult> GetForecastByZip(string zip)
    {
        var result = new ApiResult
        {
            IsSuccess = true
        };

        GeoLocation? geoLocation = await GetGeoLocation(zip);
        if (geoLocation == null)
        {
            return FailedResult($"Failed to fetch GeoLocation for zip: {zip}");
        }

        var coordinates = geoLocation.results?.FirstOrDefault();
        if (coordinates == null)
        {
            return FailedResult("GeoLocation results are empty.");
        }
        result.City = coordinates.formatted_address; 
        OfficeGridPoints? gridPoints = await GetGridPoints(
                        coordinates.geometry.Location.Lat,
                        coordinates.geometry.Location.Lng);
        if (gridPoints == null)
        {
            return FailedResult("Failed to fetch Grid Points.");
        }
        string observationStationURL = gridPoints.ObservationStations;

        var dailyForecast = await GetForecast<DailyForecast>(gridPoints.Forecast);
        if (dailyForecast == null)
        {
            return FailedResult("Failed to fetch daily forecast.");
        }
        var hourlyForecast = await GetForecast<HourlyForecast>(gridPoints.ForecastHourly);
        if (hourlyForecast == null)
        {
            return FailedResult("Failed to fetch hourly forecast.");
        }
        var currentForecast = await GetForecast<CurrentForecast>(observationStationURL);
        if (currentForecast == null)
        {
            return FailedResult("Failed to fetch current forecast.");
        }
        var currentAlerts = await GetActiveAlerts(coordinates.geometry.Location.Lat, coordinates.geometry.Location.Lng);

        result.Data = new CombinedForecast
        {
           Current = currentForecast,
           Daily = dailyForecast,
           Hourly = hourlyForecast,
           Alerts = currentAlerts
        };

        return result;
    }

    private async Task<T?> GetForecast<T>(string url)
        where T : class
    {
        var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");
        int retries = 3;

        if (typeof(T) == typeof(CurrentForecast))
        {
            var stations = await httpClient.GetFromJsonAsync<Stations>(url);
            var closestStation = stations.observationStations[0];
            string append = "/observations/latest";
            url = closestStation + append;
        }

        while (retries > 0)
        {
            try
            {
                var forecast = await httpClient.GetFromJsonAsync<T>(url);
                if (forecast != null)
                {
                    return forecast;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Will retry calling api as there was an error getting the forecast: {ex.Message}");
                retries--;

                if (retries == 0)
                {
                    _logger.LogError($"After 3 retries The API would not deliver our forecast: {ex.Message}");
                    return null;
                }
            }
        }

        return null;
    }

    private async Task<CurrentForecast?> GetCurrentForecast(string url)
    {
        var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");
        var stations = await httpClient.GetFromJsonAsync<Stations>(url);
        var closestStation = stations.observationStations[0];
        string append = "/observations/latest";
        string completeUrl = closestStation + append;
        return await httpClient.GetFromJsonAsync<CurrentForecast>(completeUrl);
    }

    private async Task<GeoLocation?> GetGeoLocation(string zip)
    {
        var httpClient = _httpClientFactory.CreateClient("GeoCode");
        var geoApiKey = _configuration["APIs:GeoCode:Key"];

        var completeUrl = $"{httpClient.BaseAddress}json?address={zip}&key={geoApiKey}";

        return await httpClient.GetFromJsonAsync<GeoLocation>(completeUrl);
    }

    private async Task<OfficeGridPoints?> GetGridPoints(double latitude, double longitude)
    {
        var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");
        var point = string.Join(",",
            latitude.ToString(CultureInfo.InvariantCulture),
            longitude.ToString(CultureInfo.InvariantCulture));
        var requestUri = $"points/{point}";

        return (await httpClient.GetFromJsonAsync<OfficeGridPointsResponse>(requestUri))?.properties;
    }

    private ApiResult FailedResult(string errorMessage)
    {
        _logger.LogError("ApplicationInsights", errorMessage);
        return new ApiResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
    private async Task<ActiveAlerts> GetActiveAlerts(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(latitude) || latitude < -90 || latitude > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
        }

        if (!double.IsFinite(longitude) || longitude < -180 || longitude > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");
        }

        var point = string.Join(",",
            latitude.ToString(CultureInfo.InvariantCulture),
            longitude.ToString(CultureInfo.InvariantCulture));
        var alertsEndpoint = _configuration["APIs:Alerts:Uri"]
            ?? throw new InvalidOperationException("Configuration value 'APIs:Alerts:Uri' is required.");
        if (!Uri.TryCreate(alertsEndpoint, UriKind.Absolute, out var alertsEndpointUri)
            || (alertsEndpointUri.Scheme != Uri.UriSchemeHttps && alertsEndpointUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("Configuration value 'APIs:Alerts:Uri' must be an absolute HTTP or HTTPS URL.");
        }

        var separator = string.IsNullOrEmpty(alertsEndpointUri.Query) ? "?" : "&";
        var requestUri = new Uri($"{alertsEndpointUri}{separator}point={point}", UriKind.Absolute);
        var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");

        try
        {
            var alerts = await httpClient.GetFromJsonAsync<ActiveAlerts>(requestUri, cancellationToken);
            return alerts ?? throw new JsonException("The active alerts response was empty.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch active weather alerts.");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize active weather alerts.");
            throw;
        }
    }
}

