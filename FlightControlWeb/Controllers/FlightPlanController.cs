using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/FlightPlan")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private FlightPlan[] FlightPlans = JsonConvert.DeserializeObject<FlightPlan[]>(System.IO.File.ReadAllText(@".\Controllers\flight-plans.json"));

        // GET: api/FlightPlan
        [HttpGet]
        public FlightPlan[] Get()
        {
            return FlightPlans;
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "GetFlightPlan")]
        public string Get(string id)
        {
            return Array.Find(FlightPlans, plan => plan.flight_id == id).ToJson();
        }

        // POST: api/FlightPlan
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/FlightPlan/5
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