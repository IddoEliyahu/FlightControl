using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FlightControl;
using Newtonsoft.Json;
using FlightControl.Models;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb

{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalServersController : ControllerBase

    {
        private readonly DataBaseContext dataBase;


        public ExternalServersController(DataBaseContext dataBaseContext)
        {
            this.dataBase = dataBaseContext;
        }


        // GET: api/Servers

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServer()
        {
            return await dataBase.Servers.ToListAsync();
        }


        // POST: api/Servers

        [HttpPost]
        public async Task<ActionResult<Server>> PostServer(Server newServer)

        {
            if (newServer.ServerURL.Last() != '/')
            {
                newServer.ServerURL += "/";
            }

            dataBase.Servers.Add(newServer);
            try
            {
                await dataBase.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (dataBase.Servers.Any(e => e.ServerID == newServer.ServerID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetServer", new {id = newServer.ServerID}, newServer);
        }
    }
}