using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using FlightControl.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControl

{

    [ApiController]

    [Route("api/[controller]")]

    public class ExternalServersController : ControllerBase

    {

        private IMemoryCache cache;

        private ServersManager serverManager;



        public ExternalServersController(IMemoryCache cache)

        {

            this.cache = cache;

            serverManager = new ServersManager(this.cache);

        }



        // GET: api/Servers

        [HttpGet]

        public ActionResult<IEnumerable<Server>> GetAllExternalServers()

        {

            return CreatedAtAction(actionName: "GetAllExternalServers", this.serverManager.getAllExternalServers());

        }



        // POST: api/Servers

        [HttpPost]

        public ActionResult AddNewServer(Server newServer)

        {

            this.serverManager.addNewServer(newServer);

            return CreatedAtAction(actionName: "AddNewServer", newServer);

        }



        // GET: api/Servers/5

        [HttpGet("{serverID}", Name = "GetServer")]



        public ActionResult<Server> GetServer(string serverID)

        {

            try

            {

                Server s = this.serverManager.getServer(serverID);

                return CreatedAtAction(actionName: "GetServer", s);

            }

            catch

            {

                return BadRequest();

            }

        }



        // DELETE: api/ApiWithActions/5

        [HttpDelete("{serverID}")]

        public ActionResult DeleteServer(string serverID)

        {

            try

            {

                this.serverManager.deleteServer(serverID);

                return CreatedAtAction(actionName: "DeleteServer", "Server with ID " + serverID + " deleted");

            }

            catch

            {

                return BadRequest();

            }

        }

    }

}