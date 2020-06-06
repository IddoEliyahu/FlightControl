using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace FlightControl.Models
{
    public class FlightPlan
    {
        [Key] public string flight_id { get; set; } = null;
        public int passengers { get; set; }
        public string company_name { get; set; }

        internal string _initial_location { get; set; }

        // [ForeignKey("flight_id")]
        [NotMapped]
        public LocationWithTime initial_location
        {
            get
            {
                return _initial_location == null
                    ? null
                    : JsonConvert.DeserializeObject<LocationWithTime>(_initial_location);
            }
            set { _initial_location = JsonConvert.SerializeObject(value); }
        }
        // [ForeignKey("flight_id")]

        // internal ICollection<Segment> segments { get; set; }
        
        internal string _segments { get; set; }

        [NotMapped]
        public Segment[] segments
        {
            get
            {
                return _segments == null
                    ? null
                    : JsonConvert.DeserializeObject<Segment[]>(_segments);
            }
            set { _segments = JsonConvert.SerializeObject(value); }
        }

        public string ToJson()
        {
            // string SegmentsString = "";
            // bool flag = true;
            // foreach (Segment s in this.segments)
            // {
            //     if (flag)
            //     {
            //         flag = false;
            //     }
            //     else
            //     {
            //         SegmentsString += ", ";
            //     }
            //
            //     SegmentsString += s.ToJson();
            // }
            //
            // return "{passengers: " + this.passengers + ", company_name: "
            //        + this.company_name + ", initial_location: " + this.initial_location.ToJson()
            //        + ", segments: [ " + SegmentsString + " ] }";

            return JsonConvert.SerializeObject(this);
        }

        private static Dictionary<string, int> nameIndexDictionary = new Dictionary<string, int>();
        public static string GenerateFlightKey()

        {
            string flight_key = "~";
            int randForChar = 0;
            Random random = new Random();
            randForChar = random.Next(0, 26);
            flight_key += ((char) ('A' + randForChar)).ToString();
            randForChar = random.Next(0, 26);
            flight_key += ((char) ('A' + randForChar)).ToString() + random.Next(0, 9).ToString() + random.Next(0,
                9).ToString() + random.Next(0, 9).ToString();
            return flight_key;
        }


        public DateTime GetLandingTime()
        {
            DateTime landingTime = initial_location.date_time;
            foreach (Segment segment in segments)
            {
                // TODO: is this really working?
                landingTime = landingTime.AddSeconds(segment.timespan_seconds);
            }

            return landingTime;
        }

        public Tuple<LocationWithTime, Segment> FindRelevantSegments(DateTime relativeTo)
        {
            Segment relevantSegment;
            LocationWithTime totalFlightTime = initial_location;
            for (int i = 0; i < segments.Length; i++)
            {
                if (totalFlightTime.date_time.AddSeconds(segments[i].timespan_seconds) > relativeTo)
                {
                    return new Tuple<LocationWithTime, Segment>(totalFlightTime, segments[i]);
                }

                totalFlightTime.date_time = totalFlightTime.date_time.AddSeconds(segments[i].timespan_seconds);
                totalFlightTime.latitude = segments[i].latitude;
                totalFlightTime.longitude = segments[i].longitude;
            }

            Console.WriteLine("Segment not found, relativeTo : " + relativeTo);
            return null;
        }

        public Flight CreateFlightJson(LocationWithTime currentLocation)
        {
            return (new Flight(currentLocation, this, false));
        }
    }

    public class Segment
    {
        // [Key]
        // public string segment_id { get; set; }
        // public string flight_id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int timespan_seconds { get; set; }

        public string ToJson()
        {
            return "{ longitude: " + this.longitude + ", latitude: " + this.latitude + " timespan_seconds: " +
                   this.timespan_seconds + "}";
        }
    }
}