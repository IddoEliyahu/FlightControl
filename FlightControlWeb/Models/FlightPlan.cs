using System;


namespace FlightControl.Models
{
    public class FlightPlan
    {
        public string flight_id { get; set; }
        public int passengers { get; set; }
        public string company_name { get; set; }
        public LocationWithTime initial_location { get; set; }
        public Segment[] segments { get; set; }

        public string ToJson()
        {
            string SegmentsString = "";
            bool flag = true;
            foreach (Segment s in this.segments)
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

            return "{ flight_id: " + this.flight_id + ", passengers: " + this.passengers + ", company_name: "
                   + this.company_name + ", initial_location: " + this.initial_location.ToJson()
                   + ", segments: [ " + SegmentsString + " ] }";
        }
    }

    public class Segment
    {
        public double longitude { get; set; }

        public double latitude { get; set; }
        public int timespan_seconds { get; set; }

        public string ToJson()
        {
            return "{ longitude: " + this.longitude + ", latitude: " + this.latitude + " timespan_seconds: " +
                   this.timespan_seconds + "}";
        }
    }

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