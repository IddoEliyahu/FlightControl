using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControl.Models
{
    
    public class Flight
    {
        [Key]
        public string flight_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        
        public DateTime date_time { get; set; }

        public bool isExternal { get; set; }




        public int passengers { get; set; }

        public string company_name { get; set; }

        public string ToJson()
        {
            return "{ flight_id:" + this.flight_id + ", "
                   + "latitude: " + this.latitude + ", longitude: " + this.longitude + ", passengers: " + this.passengers
                   + ", company_name: " + this.company_name + ", date_time: " + this.date_time.ToLongDateString()
                   + "is_external: " + this.isExternal + " }";
        }
    }
}