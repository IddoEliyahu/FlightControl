using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class ServersManager
    {

            private IMemoryCache cache;

            public ServersManager(IMemoryCache cache)

            {

                this.cache = cache;

            }



            // Servers method implemantaion

            public void addNewServer(Server newServer)

            {

                // get the list from the cache

                var serversList = ((IEnumerable<Server>)cache.Get("servers")).ToList();

                serversList.Add(newServer);

                // insert the list to the cache

                cache.Set("servers", serversList);

            }



            public IEnumerable<Server> getAllExternalServers()

            {

                // get the list from the cache

                var serversList = ((IEnumerable<Server>)cache.Get("servers")).ToList();
                return serversList;

            }



            public void deleteServer(string serverID)

            {

                // get the list from the cache
                var serversList = ((IEnumerable<Server>)cache.Get("servers")).ToList();


                Server getServer = serversList.Where(x => String.Equals(x.ServerID, serverID)).FirstOrDefault();



                // insert the list to the cache

                cache.Set("servers", serversList);



                if (getServer != null)

                    serversList.Remove(getServer);

                else

                    throw new Exception("Server does not exist");

            }



            public Server getServer(string serverID)

            {

                // get the list from the cache

                var serversList = ((IEnumerable<Server>)cache.Get("servers")).ToList();

                Server getServer = serversList.Where(x => String.Equals(x.ServerID, serverID)).FirstOrDefault();

                // insert the list to the cache

                cache.Set("servers", serversList);



                if (getServer == null)

                    throw new Exception("Server does not exist");

                else

                    return getServer;

            }

        }
}
