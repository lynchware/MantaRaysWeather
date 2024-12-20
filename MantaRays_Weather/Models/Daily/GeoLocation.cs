﻿namespace MantaRays_Weather.Models.Daily
{
    public class GeoLocation
    {
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] postcode_localities { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public Location Location { get; set; }
    }

    public class Location
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
    }
}


