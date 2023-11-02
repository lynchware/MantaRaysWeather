namespace MantaRays_Weather.Models
{
    public class ApiResult<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? City { get; set; }
    }
}
