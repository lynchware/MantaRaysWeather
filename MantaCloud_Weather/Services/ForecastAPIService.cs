using MantaRays_Weather.Interfaces;
using System.Net.Http;
using MantaRays_Weather.Models.Daily;
using MantaRays_Weather.Models.Hourly;
using MantaRays_Weather.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using MantaRays_Weather.Models;

namespace MantaRays_Weather.Services
{
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

        public async Task<ApiResult<T?>> GetForecastByZip<T>(string zip, ForecastType type)
            where T : class
        {
            var result = new ApiResult<T>
            {
                IsSuccess = true
            };

            GeoLocation? geoLocation = await GetGeoLocation(zip);
            if (geoLocation == null)
            {
                return FailedResult<T>($"Failed to fetch GeoLocation for zip: {zip}");
            }

            var coordinates = geoLocation.results?.FirstOrDefault();
            if (coordinates == null)
            {
                return FailedResult<T>("GeoLocation results are empty.");
            }
            result.City = coordinates.formatted_address; 
            OfficeGridPoints? gridPoints = await GetGridPoints(
                            coordinates.geometry.Location.Lat.ToString(),
                            coordinates.geometry.Location.Lng.ToString());
            if (gridPoints == null)
            {
                return FailedResult<T>("Failed to fetch Grid Points.");
            }

            string forecastUrl = type == ForecastType.Daily ? gridPoints.Forecast : gridPoints.ForecastHourly;
            
            var forecastResult = await GetForecast<T>(forecastUrl);
            if (forecastResult == null)
            {
                return FailedResult<T>($"Failed to fetch {type} forecast after 3 unsuccessful attempts");
            }
            result.Data = forecastResult;

            return result;
        }

        private async Task<T?> GetForecast<T>(string url)
            where T : class
        {
            var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");
            int retries = 3;

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

        private async Task<GeoLocation?> GetGeoLocation(string zip)
        {
            var httpClient = _httpClientFactory.CreateClient("GeoCode");
            var geoApiKey = _configuration["APIs:GeoCode:Key"];

            var completeUrl = $"{httpClient.BaseAddress}json?address={zip}&key={geoApiKey}";

            return await httpClient.GetFromJsonAsync<GeoLocation>(completeUrl);
        }

        private async Task<OfficeGridPoints?> GetGridPoints(string lat, string lon)
        {
            var httpClient = _httpClientFactory.CreateClient("NWSPointLocation");
            var completeUrl = $"{httpClient.BaseAddress}{lat},{lon}";

            return (await httpClient.GetFromJsonAsync<OfficeGridPointsResponse>(completeUrl))?.properties;
        }


        private ApiResult<T> FailedResult<T>(string errorMessage) where T : class
        {
            _logger.LogError("ApplicationInsights", errorMessage);
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}

