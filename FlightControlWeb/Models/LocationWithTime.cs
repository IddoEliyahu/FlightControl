using System;

namespace FlightControlWeb.Models
{
    public class LocationWithTime
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public DateTime date_time { get; set; }

        public string ToJson()
        {
            return "{ longitude: " + this.longitude + ", latitude: " + this.latitude + ", date_time: " +
                   this.date_time.ToLongDateString() + "}";
        }
    }
}