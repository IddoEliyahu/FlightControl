using System;
using System.Collections.Generic;
using FlightControl.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControl.Controllers
{
    [Route("api/Flights/")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private Flight[] Flights = JsonConvert.DeserializeObject<Flight[]>(System.IO.File.ReadAllText(@".\Controllers\flights.json"));

        // GET: api/Flights
        [HttpGet]
        public Flight[] Get()
        {
            return this.Flights;
        }

        // GET: api/Flights/5
        [HttpGet("{id}", Name = "GetFlight")]
        public string Get(string id)
        {
            return Array.Find(Flights, flight => flight.flight_id == id).ToJson();
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
