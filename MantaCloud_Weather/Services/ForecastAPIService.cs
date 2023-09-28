using MantaRays_Weather.Interfaces;
using System.Net.Http;
using MantaRays_Weather.Models.Daily;
using MantaRays_Weather.Models.Hourly;
using MantaRays_Weather.Enums;

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

        public async Task<T> GetForecastByZip<T>(string zip, ForecastType type)
        {
            GeoLocation geoLocation = await GetGeoLocation(zip);
            if (geoLocation == null)
            {
                throw new Exception("Failed to fetch GeoLocation.");
            }

            var coordinates = geoLocation.results?.FirstOrDefault();
            if (coordinates == null)
            {
                throw new Exception("GeoLocation results are empty.");
            }

            OfficeGridPoints gridPoints = await GetGridPoints(
                            coordinates.geometry.Location.Lat.ToString(),
                            coordinates.geometry.Location.Lng.ToString());
            if (gridPoints == null)
            {
                throw new Exception("Failed to fetch Grid Points.");
            }

            string forecastUrl = type == ForecastType.Daily ? gridPoints.Forecast : gridPoints.ForecastHourly;
            T forecast = await GetForecast<T>(forecastUrl);
            if (forecast == null)
            {
                throw new Exception("Failed to fetch Weather Forecast.");
            }

            return forecast;
        }

        private async Task<T> GetForecast<T>(string url)
        {
            T forecast = default(T);
            var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");

            try
            {
                forecast = await httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our forecast: {ex.Message}");
            }

            return forecast;
        }   

        private async Task<GeoLocation> GetGeoLocation(string zip)
        {
            GeoLocation geoLocation = null;
            var httpClient = _httpClientFactory.CreateClient("GeoCode");
            var geoApiKey = _configuration["APIs:GeoCode:Key"];

            var completeUrl = $"{httpClient.BaseAddress}json?address={zip}&key={geoApiKey}";

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

        private async Task<OfficeGridPoints> GetGridPoints(string lat, string lon)
        {
            OfficeGridPointsResponse response = new(); //Forecast & hourly forecast urls are in this response

            var httpClient = _httpClientFactory.CreateClient("NWSPointLocation");
            var completeUrl = $"{httpClient.BaseAddress}{lat},{lon}";
            try
            {
                response = await httpClient.GetFromJsonAsync<OfficeGridPointsResponse>(completeUrl);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our grid points: {ex.Message}");
            }

            return response.properties;
        }

        //I may not need this as the data returned from the grid points includes the forecast urls. 
        private async Task<DailyForecast> GetDailyForecast(string url)
        {
            DailyForecast forecast = null;
            var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");

            try
            {
                forecast = await httpClient.GetFromJsonAsync<DailyForecast>(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our forecast: {ex.Message}");
            }

            return forecast;
        }
       
        private async Task<HourlyForecast> GetHourlyForecast(string url)
        {
            HourlyForecast forecast = null;
            var httpClient = _httpClientFactory.CreateClient("NationalWeatherService");

            try
            {
                forecast = await httpClient.GetFromJsonAsync<HourlyForecast>(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error getting our forecast: {ex.Message}");
            }

            return forecast;
        }


    }
}
