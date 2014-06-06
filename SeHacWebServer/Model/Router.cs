using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    public abstract class Router
    {
        private ResponseStatus response { get; set; }
        private Server server;

        public Router(Server server)
        {
            this.server = server;
        }

        public abstract string CheckRoutes(string url, string host);
    }
}
