using MantaRays_Weather.Models;
using MantaRays_Weather.Interfaces;
using System.Net.Http;

namespace MantaRays_Weather.Services
{
    public class ForecastAPIService : IForecastAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _geoApiKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ForecastAPIService(HttpClient httpClient, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _geoApiKey = configuration["APIs:GeoCode:Key"];
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<GeoLocation> GetGeoLocation(string zip)
        {
            GeoLocation geoLocation = null;
            var httpClient = _httpClientFactory.CreateClient("GeoCode");
            var geoApiKey = _configuration["APIs:GeoCode:Key"];

            var completeUrl = $"json?address={zip}&key={geoApiKey}";

            try
            {
                geoLocation = await httpClient.GetFromJsonAsync<GeoLocation>(completeUrl);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our GeoLocation: {ex.Message}");
            }

            return geoLocation;
        }

        public async Task<OfficeGridPoints> GetGridPointsAsync(string lat, string lon)
        {
            OfficeGridPoints gridPoints = null;
            var httpClient = _httpClientFactory.CreateClient("NWSPointLocation");

            try
            {
                gridPoints = await httpClient.GetFromJsonAsync<OfficeGridPoints>($"points/{lat},{lon}");
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our grid points: {ex.Message}");
            }

            return gridPoints;
        }

        public async Task<WeatherForecast> GetForecastAsync(string office, string gridx, string gridy)
        {
            WeatherForecast forecast = null;
            var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");

            try
            {
                forecast = await httpClient.GetFromJsonAsync<WeatherForecast>($"{office}/{gridx},{gridy}/forecast");
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our forecast: {ex.Message}");
            }

            return forecast;
        }

    }
}
