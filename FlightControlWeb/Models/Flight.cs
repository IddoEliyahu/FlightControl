using System;
using System.ComponentModel.DataAnnotations;
using FlightControlWeb.Models;

namespace FlightControl.Models
{
    
    public class Flight
    {
        public Flight()
        {
            
        }
        public Flight(FlightPlan flightPlan, bool isExternal)
        {
            Initialize(flightPlan, isExternal);
        }

        public Flight(LocationWithTime locationWithTime, FlightPlan flightPlan, bool isExternal)
        {
            
            longitude = locationWithTime.longitude;
            latitude = locationWithTime.latitude;
            date_time = locationWithTime.date_time;

            Initialize(flightPlan, isExternal);
        }
        
        private void Initialize(FlightPlan flightPlan, bool isExternal)
        {
            this.flight_id = flightPlan.flight_id;
            this.company_name = flightPlan.company_name;
            this.passengers = flightPlan.passengers;
            this.is_external = isExternal;
        }
        
        [Key]
        public string flight_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        
        public DateTime date_time { get; set; }

        public bool is_external { get; set; }
        
        public int passengers { get; set; }

        public string company_name { get; set; }

        public string ToJson()
        {
            return "{ flight_id:" + this.flight_id + ", "
                   + "latitude: " + this.latitude + ", longitude: " + this.longitude + ", passengers: " + this.passengers
                   + ", company_name: " + this.company_name + ", date_time: " + this.date_time.ToLongDateString()
                   + "is_external: " + this.is_external + " }";
        }
    }
}