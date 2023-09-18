namespace MantaRays_Weather.Models.Daily
{
    public class OfficeGridPointsResponse
    {
        public OfficeGridPoints properties { get; set; }
    }
    public class OfficeGridPoints
    {
        public string ForecastOffice { get; set; }
        public string GridId { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        public string Forecast { get; set; }
        public string ForecastHourly { get; set; }
        public string ForecastGridData { get; set; }
        public string ObservationStations { get; set; }
    }

}
