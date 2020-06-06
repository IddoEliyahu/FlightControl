using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text.Json;
using FlightControl;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

namespace FlightControlWeb.Controllers
{
    [Route("api/FlightPlan")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly DataBaseContext database;

        public FlightPlanController(DataBaseContext context)
        {
            this.database = context;
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}", Name = "GetFlightPlan")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            var flightsAsSegments = database.FlightPlans.Include(x => x.segments);

            var requestedId = flightsAsSegments.Include(x => x.initial_location).Where(x =>
                String.Equals(id, x.flight_id));

            var flightPlan = await requestedId.FirstOrDefaultAsync();

            if (flightPlan != null)

            {
                return flightPlan;
            }

            var address = await Task.Run(() => FindFlightServer(id));

            var response = await Task.Run(() => GetFlightPlanFromExtServer(address, id));

            try

            {
                flightPlan = ParseFlightPlanFromResponse(response);

                if (flightPlan.flight_id.Equals("null"))

                {
                    flightPlan.flight_id = id;
                }

                return flightPlan;
            }

            catch

            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(JsonElement s)
        {
            FlightPlan flightPlan = JsonConvert.DeserializeObject<FlightPlan>(s.ToString());
            if (!s.ToString().Contains("flight_id"))
            {
                do
                {
                    flightPlan.flight_id = FlightPlan.GenerateFlightKey();
                    //As long as generated key is identical to a key in DB, generate a new one.
                } while (database.FlightPlans.Count(x => x.flight_id == flightPlan.flight_id) > 0);
            }

            database.FlightPlans.Add(flightPlan);

            //Generate a unique key for new flight.


            await database.SaveChangesAsync();

            return CreatedAtAction("GetFlightPlan", new {id = flightPlan.flight_id}, flightPlan);
        }


        private bool FlightPlanExists(string id)

        {
            return database.FlightPlans.Any(e => String.Equals(e.flight_id, id));
        }

        private FlightPlan ParseFlightPlanFromResponse(HttpWebResponse response)
        {
            string jsonText = null;

            //Creating a stream object from external API response.

            using (Stream stream = response.GetResponseStream())

            {
                StreamReader sr = new StreamReader(stream);

                //Assigning JSON from external API to a string.

                jsonText = sr.ReadToEnd();

                sr.Close();
            }

            var deserialSettings = new JsonSerializerSettings

            {
                ContractResolver = new DefaultContractResolver

                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var flightPlan = JsonConvert.DeserializeObject<FlightPlan>(jsonText, deserialSettings);

            return flightPlan;
        }

        private HttpWebResponse GetFlightPlanFromExtServer(object address, string id)
        {
            var fullAddress = address + "api/FlightPlans/" + id;

            string extURL = String.Format(fullAddress);

            WebRequest request = WebRequest.Create(extURL);

            request.Method = "GET";

            HttpWebResponse response = null;

            //Getting a response from external API.

            response = (HttpWebResponse) request.GetResponse();

            return response;
        }

        private string FindFlightServer(string id)
        {
            var serverURL = database.ServersById.Find(id);

            return serverURL.ServerURL;
        }

        //// PUT: api/FlightPlans/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}