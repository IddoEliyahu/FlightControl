using System;
using System.Collections.Generic;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private Flight[] Flights = JsonConvert.DeserializeObject<Flight[]>(System.IO.File.ReadAllText(@".\Controllers\flights.json"));

        // GET: api/Flights
        [HttpGet]
        public Flight[] Get(DateTime? relative_to, bool? sync_all)
        {
            return this.Flights;
        }

        // GET: api/Flights/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(string id)
        {
            return Array.Find(Flights, Flight => Flight.flight_id == id).ToJson();
        }

        // POST: api/Flights
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
