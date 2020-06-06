using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using FlightControl.Models;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlightControl.Controllers
{
    [Route("api/Flights/")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly DataBaseContext database;

        public FlightsController(DataBaseContext db)

        {
            database = db;
        }

        private Flight[] Flights =
            JsonConvert.DeserializeObject<Flight[]>(System.IO.File.ReadAllText(@".\Controllers\flights.json"));
        // TODO: Flights should be database.Flights

        // GET: api/Flights
        [HttpGet]
        public Flight[] Get(DateTime? relative_to)
        {
            List<Flight> flightsToReturn = new List<Flight>();
            DateTime relativeTo;
            if (relative_to == null)
            {
                relativeTo = DateTime.Now;
            }
            else
            {
                relativeTo = relative_to.Value;
            }

            foreach (FlightPlan flightPlan in database.FlightPlans)
            {
                if (relativeTo > flightPlan.initial_location.date_time &&
                    relativeTo < flightPlan.GetLandingTime())
                {
                    Tuple<LocationWithTime, Segment> segments = flightPlan.FindRelevantSegments(relativeTo);

                    if (segments != null)
                    {
                        LocationWithTime startSeg = segments.Item1;
                        Segment endSeg = segments.Item2;
                        TimeSpan timeOnSegment = relativeTo.Subtract(startSeg.date_time);
                        int time = (int) timeOnSegment.TotalSeconds;
                        double latSlope = (endSeg.latitude - startSeg.latitude) / endSeg.timespan_seconds,
                            lonSlope = (endSeg.longitude - startSeg.longitude) / endSeg.timespan_seconds;
                        startSeg.latitude += time * latSlope;
                        startSeg.longitude += time * lonSlope;

                        startSeg.date_time = relativeTo;

                        flightsToReturn.Add(flightPlan.CreateFlightJson(startSeg));
                    }
                }
            }
            return flightsToReturn.ToArray();
        }

        // GET: api/Flights/5
        [HttpGet("{id}", Name = "GetFlight")]
        public string Get(string id)
        {
            return Array.Find(Flights, flight => flight.flight_id == id).ToJson();
        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlight(string id)
        {
            try
            {
                var flightPlan = await database.FlightPlans.FindAsync(id);
                if (flightPlan == null)
                {
                    return NotFound();
                }

                database.FlightPlans.Remove(flightPlan);
                await database.SaveChangesAsync();
                return flightPlan;
            }
            catch
            {
                return BadRequest();
            }
        }

        public List<Flight> HandleExternalServers(DateTime time)
        {
            List<Flight> ExternalFlights = new List<Flight>();

            var serversList = database.Servers.Select(x => x.ServerURL).ToList();
            foreach (string address in serversList)
            {
                ExternalFlights.AddRange(GetFlightsFromExtServer(address, time));
            }

            return ExternalFlights;
        }


        public List<Flight> GetFlightsFromExtServer(string address, DateTime time)
        {
            List<Flight> ExternalFlights = new List<Flight>();
            string addresstocall = address + "api/Flights/?relative_to=" + time.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string jsonText = GetJsonFromServer(addresstocall);

            var deserialSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            List<Flight> externalserverflights;
            try

            {
                externalserverflights = JsonConvert.DeserializeObject<List<Flight>>(jsonText, deserialSettings);
            }
            catch
            {
                return ExternalFlights;
            }

            //Update flight as external and add to all active flights list.
            foreach (Flight flight in externalserverflights)

            {
                flight.is_external = true;

                ExternalFlights.Add(flight);

                if (!database.ServersById.Any(e => e.FlightID.Equals(flight.flight_id)))

                {
                    var serverbyidr = new ServerById();

                    serverbyidr.FlightID = flight.flight_id;

                    serverbyidr.ServerURL = address;

                    database.ServersById.Add(serverbyidr);
                }
            }

            database.SaveChanges();
            return ExternalFlights;
        }

        public string GetJsonFromServer(string address)
        {
            string extURL = String.Format(address);
            WebRequest request = WebRequest.Create(extURL);
            request.Method = "GET";
            HttpWebResponse response = null;
            response = (HttpWebResponse) request.GetResponse();

            string jsonText = null;
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                jsonText = sr.ReadToEnd();
                sr.Close();
            }

            return jsonText;
        }
    }
}