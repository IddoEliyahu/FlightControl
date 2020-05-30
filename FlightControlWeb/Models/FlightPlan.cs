using System;
using Microsoft.CodeAnalysis.CSharp;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Passengers { get; set; }
        public string CompanyName { get; set; }
        public LocationWithTime InitialLocation { get; set; }
        public Segment[] Segments { get; set; }

        public string ToJson()
        {
            string SegmentsString = "";
            bool flag = true;
            foreach (Segment s in this.Segments)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    SegmentsString += ", ";
                }
                
                SegmentsString += s.ToJson();
            }

            return "{ passengers: " + this.Passengers + ", company_name: "
                   + this.CompanyName + ", initial_location: " + this.InitialLocation.ToJson()
                   + ", segments: [ " + SegmentsString + " ] }";
        }
    }

    public class Segment
    {
        public GeoLocation Location { get; set; }
        public int TimespanSeconds { get; set; }

        public string ToJson()
        {
            return "{ " + this.Location.ToJson() + ", timespan_seconds: " + this.TimespanSeconds + "}";
        }
    }
    
    public class GeoLocation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public string ToJson()
        {
            return "longitude: " + this.Longitude + ", latitude: " + Latitude;
        }
    }

    public class LocationWithTime
    {
        public GeoLocation Location { get; set; }
        public DateTime Time { get; set; }

        public string ToJson()
        {
            return "{ " + this.Location.ToJson() + ", date_time: " + this.Time.ToLongDateString() + "}";
        }
    }
}