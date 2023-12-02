namespace MantaRays_Weather.Models
{
    public class ApiResult
    {
        public CombinedForecast? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? City { get; set; }
    }
}
