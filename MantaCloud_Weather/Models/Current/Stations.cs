namespace MantaRays_Weather.Models.Current
{

    public class Stations
    {
        public string[] observationStations { get; set; }
        //This returns a list of observation stations. return the first one and then append /observations/latest to the end to get current forecast.
    }

}
